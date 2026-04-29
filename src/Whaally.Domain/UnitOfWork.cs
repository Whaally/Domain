using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class UnitOfWork(IServiceProvider services) : IUnitOfWork
{
    private readonly DomainContext _domainContext = services.GetRequiredService<DomainContext>();
    private readonly IContextFactory _contextFactory = services.GetRequiredService<IContextFactory>();
    private readonly IAggregateHandlerFactory _handlerFactory = services.GetRequiredService<IAggregateHandlerFactory>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceEnvelope"></param>
    /// <returns></returns>
    public async Task<IResult<CommandEnvelope[]>> Evaluate(ServiceEnvelope serviceEnvelope)
    {
        // TODO: Can we support evaluation of multiple services? What does this mean for the transactional boundaries?
        using var serviceContext = _contextFactory.CreateServiceHandlerContext(serviceEnvelope.Metadata);
        
        var output = await _domainContext
            .GetServiceHandler(serviceEnvelope.Message.GetType())
            .Invoke(
                serviceContext,
                serviceEnvelope.Message);

        // Go through this services' output and evaluate all returned services to the commands they intend to invoke.
        var intermediate = 
            (await Task.WhenAll(
                output.Operations
                    .Select(async q => q switch
                    {
                        ServiceEnvelope s => await Evaluate(s),
                        CommandEnvelope c => new Result<CommandEnvelope[]>([ c ]),
                        _ => throw new InvalidOperationException()
                    })))
            .Select(q => q)
            .ToList();

        // Todo: merge various CommandEnvelopes based on target type and id
        return new Result<CommandEnvelope[]>(
            intermediate
                .SelectMany(q => q.Value ?? throw new ArgumentException())
                .ToArray(),
            intermediate.SelectMany(q => q.Errors));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="commandEnvelopes"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<IResult<EventEnvelope[]>> Evaluate(params CommandEnvelope[] commandEnvelopes)
    {
        // Todo: merge command envelopes for objects with the same id / type
        // todo: pre-emptively validate the provided DTOs using their validation attributes
        
        List<IResult<EventEnvelope>> results = [];
        
        await Parallel.ForEachAsync(commandEnvelopes, async (envelope, ct) =>
        {
            if (!envelope.Messages.Any()) return;

            foreach (var message in envelope.Messages)
            {
                var context = new ValidationContext(message);
                var validationResults = new Collection<ValidationResult>();
                Validator.TryValidateObject(message, context, validationResults);
            }
            
            if (envelope.Metadata.AggregateType == null)
                envelope.Metadata.AggregateType = _domainContext.GetCommonAggregateType(envelope.Messages);
            
            var handler = await _handlerFactory.Instantiate(
                envelope.Metadata.AggregateType,
                envelope.Metadata.AggregateId);
            
            results.Add(await handler.Evaluate(envelope));
        });

        return new Result<EventEnvelope[]>(
            results
                .Select(q => q.ValueOrDefault)
                .OfType<EventEnvelope>()
                .ToArray(),
            results.SelectMany(q => q.Errors));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventEnvelopes"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<IResult> Apply(params EventEnvelope[] eventEnvelopes)
    {
        List<IResult> results = new(eventEnvelopes.Length);

        await Parallel.ForEachAsync(eventEnvelopes, async (envelope, ct) =>
        {
            if (!envelope.Messages.Any()) return;
            
            var aggregateType = _domainContext.GetCommonAggregateType(envelope.Messages);

            var handler = await _handlerFactory.Instantiate(
                aggregateType,
                envelope.Metadata.AggregateId);
            
            results.Add(await handler.Apply(envelope));
        });
        
        return new Result(results.SelectMany(q => q.Errors));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventEnvelope"></param>
    /// <returns></returns>
    public Task<IResult> Continue(EventEnvelope eventEnvelope)
    {
        foreach (var @event in eventEnvelope.Messages)
        {
            foreach (var saga in _domainContext.GetSaga(@event.GetType()))
            {
                // Does this help prevent mutation of the original?
                _ = Task.Run(() => Invoke(saga, new EventEnvelope(eventEnvelope.Metadata, @event)));
            }
        }
        
        return Task.FromResult<IResult>(Result.Success());
    }
    
    public Task Abort(params CommandMetadata[] metadata)
    {
        // TODO: Get an aggregate handler instance. Note that the aggregate type is required to do so.
        // TODO: Create an `IAggregateMetadata` object, the `ICommandMetadata` and `IEventMetadata` derive from
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="saga"></param>
    /// <param name="eventEnvelope"></param>
    /// <returns></returns>
    public async Task<IResult> Invoke(ISaga saga, EventEnvelope eventEnvelope)
    {
        if (eventEnvelope.Messages.Count() != 1)
            throw new ArgumentException($"Expected {nameof(eventEnvelope)} to contain one message");
        
        using var activity = DomainContext.ActivitySource.StartActivity(
            ActivityKind.Internal,
            name: $"Invoke {saga.GetType().Name}",
            parentContext: default,
            links: [ new ActivityLink(eventEnvelope.Metadata.ParentContext ?? default) ],
            tags: new Dictionary<string, object?>());
        
        eventEnvelope.Metadata.ParentContext = activity?.Context;

        var context = _contextFactory.CreateSagaContext(eventEnvelope.Metadata);
        var output = await saga.Evaluate(
            context,
            eventEnvelope.Messages.Single());
        
        // todo: ensure services are in fact fully evaluated
        // todo: allow to manage the whole lifecycle from this class. I.e. including service invocation
        var intermediate = 
            (await Task.WhenAll(
                output.Operations
                    .Select(async q => q switch
                    {
                        ServiceEnvelope s => await Evaluate(s),
                        CommandEnvelope c => new Result<CommandEnvelope[]>([ c ]),
                        _ => throw new InvalidOperationException()
                    })))
            .Select(q => q)
            .ToList();

        return new Result(intermediate.SelectMany(q => q.Errors));
    }
    
    public void Dispose()
    {
    }
}
