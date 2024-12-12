using System.Diagnostics;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class DefaultEvaluationAgent : IEvaluationAgent
{
    private readonly IServiceProvider _services;
    private readonly DomainContext _domainContext;
    private readonly IContextFactory _contextFactory;
    private readonly IAggregateHandlerFactory _handlerFactory;
    
    public DefaultEvaluationAgent(IServiceProvider services)
    {
        _services = services;
        _domainContext = _services.GetRequiredService<DomainContext>();
        _contextFactory = _services.GetRequiredService<IContextFactory>();
        _handlerFactory = _services.GetRequiredService<IAggregateHandlerFactory>();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceEnvelope"></param>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    public async Task<IResult<ICommandEnvelope[]>> Evaluate(IServiceEnvelope serviceEnvelope)
    {
        // TODO: Can we support evaluation of multiple services? What does this mean for the transactional boundaries?
        if (serviceEnvelope.Messages.Count() != 1)
            throw new ArgumentException($"Expected {nameof(serviceEnvelope)} to contain one message");
        
        using var serviceContext = _contextFactory.CreateServiceHandlerContext(serviceEnvelope.Metadata);
        
        var result = await _domainContext
            .GetServiceHandler(serviceEnvelope.Messages.Single().GetType())
            .Handle(
                serviceContext,
                serviceEnvelope.Message);
        
        return new Result<ICommandEnvelope[]>()
            .WithValue(serviceContext.Commands.ToArray())
            .WithReasons(result.Reasons);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="commandEnvelopes"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<IResult<IEventEnvelope[]>> Evaluate(params ICommandEnvelope[] commandEnvelopes)
    {
        List<IResult<IEventEnvelope>> results = [];

        await Parallel.ForEachAsync(commandEnvelopes, async (envelope, ct) =>
        {
            if (!envelope.Messages.Any()) return;

            if (envelope.Metadata.AggregateType == null)
                envelope.Metadata.AggregateType = _domainContext.GetCommonAggregateType(envelope.Messages);
            
            var handler = _handlerFactory.Instantiate(
                envelope.Metadata.AggregateType,
                envelope.Metadata.AggregateId);
            
            results.Add(await handler.Evaluate(envelope));
        });
        
        return new Result<IEventEnvelope[]>()
            .WithValue(results
                .Select(q => q.Value)
                .ToArray())
            .WithReasons(results.SelectMany(q => q.Reasons));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventEnvelopes"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<IResultBase> Apply(params IEventEnvelope[] eventEnvelopes)
    {
        List<IResultBase> results = new(eventEnvelopes.Length);

        await Parallel.ForEachAsync(eventEnvelopes, async (envelope, ct) =>
        {
            if (!envelope.Messages.Any()) return;
            
            var aggregateType = _domainContext.GetCommonAggregateType(envelope.Messages);

            var handler = _handlerFactory.Instantiate(
                aggregateType,
                envelope.Metadata.AggregateId);
            
            results.Add(await handler.Apply(envelope));
        });
        
        return new Result()
            .WithReasons(results.SelectMany(q => q.Reasons));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventEnvelope"></param>
    /// <returns></returns>
    public Task<IResultBase> Continue(IEventEnvelope eventEnvelope)
    {
        foreach (var @event in eventEnvelope.Messages)
        {
            foreach (var saga in _domainContext.GetSaga(@event.GetType()))
            {
                // Does this help prevent mutation of the original?
                _ = Task.Run(() => Invoke(saga, new EventEnvelope(eventEnvelope.Metadata, @event)));
            }
        }
        
        return Task.FromResult<IResultBase>(Result.Ok());
    }
    
    public Task Abort(params ICommandMetadata[] metadata)
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
    public async Task<IResultBase> Invoke(ISaga saga, IEventEnvelope eventEnvelope)
    {
        if (eventEnvelope.Messages.Count() != 1)
            throw new ArgumentException($"Expected {nameof(eventEnvelope)} to contain one message");
        
        using var activity = DomainContext.ActivitySource.StartActivity(
            ActivityKind.Internal,
            name: $"Invoke {saga.GetType().Name}",
            parentContext: default,
            links: [ new ActivityLink(eventEnvelope.Metadata.ParentContext ?? default) ],
            tags: new Dictionary<string, object?>
            {
                
            });
        
        eventEnvelope.Metadata.ParentContext = activity?.Context;
        
        return await saga.Evaluate(
            _contextFactory.CreateSagaContext(eventEnvelope.Metadata), 
            eventEnvelope.Messages.Single());
    }
    
    public void Dispose()
    {
    }
}
