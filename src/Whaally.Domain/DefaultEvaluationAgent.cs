using System.Diagnostics;
using System.Numerics;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class DefaultEvaluationAgent : IEvaluationAgent, IDisposable
{
    private readonly IServiceProvider _services;
    private readonly DomainContext _domainContext;
    private readonly IAggregateHandlerFactory _handlerFactory;
    private readonly Activity? _activity;
    
    public DefaultEvaluationAgent(IServiceProvider services)
    {
        _services = services;
        _domainContext = _services.GetRequiredService<DomainContext>();
        _handlerFactory = _services.GetRequiredService<IAggregateHandlerFactory>();

        _activity = DomainContext.ActivitySource.StartActivity(
            ActivityKind.Internal,
            name: "DomainContext",
            tags: new Dictionary<string, object?> { });
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceEnvelope"></param>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    public async Task<IResult<ICommandEnvelope[]>> Evaluate<TService>(IServiceEnvelope<TService> serviceEnvelope)
        where TService : class, IService
    {
        var serviceHandler = (IServiceHandler<TService>)_services.GetRequiredService(
            _domainContext.ServiceHandlers
                .Single(q => q.ServiceType == typeof(TService))
                .HandlerType);
        
        var serviceContext =
            new ServiceHandlerContext(_services, this)
            {
                ParentContext = _activity?.Context
            };
        
        var result = await serviceHandler.Handle<TService>(
            serviceContext,
            serviceEnvelope.Message);
        
        if (result.IsFailed)
        {
            return Result.Fail<ICommandEnvelope[]>(result.Errors);
        }
        
        return Result
            .Ok(Array.Empty<ICommandEnvelope>())
            .WithReasons(result.Reasons)
            .WithValue(serviceContext.Commands.ToArray());
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="commandEnvelopes"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<IResult<IEventEnvelope[]>> Evaluate(params ICommandEnvelope[] commandEnvelopes)
    {
        // group commands by aggregate type and id to batch operations
        var commandCollections = commandEnvelopes
            .GroupBy(q => (
                aggregateType: _domainContext.CommandHandlers
                    .Single(w => w.CommandType == q.Message.GetType())
                    .AggregateType,
                aggregateId: q.Metadata.AggregateId
            ))
            .Select(q => (
                q.Key.aggregateType,
                q.Key.aggregateId,
                commands: q.Select(w => w).ToArray(),
                result: new Result<IEventEnvelope[]>() as IResult<IEventEnvelope[]>
            ))
            .ToList();

        List<IResult<IEventEnvelope[]>> results = new(commandCollections.Count);

        foreach (var operation in commandCollections)
        {
            // using var activity = DomainContext.ActivitySource.StartActivity(
            //     ActivityKind.Internal,
            //     name: $"evaluate {operation.aggregateType!.Name}",
            //     parentContext: _activity?.Context ?? default,
            //     tags: new Dictionary<string, object?>
            //     {
            //         { "messaging.batch.message_count", operation.commands.Length },
            //         { "messaging.batch.types", $"[{string.Join(';', operation.commands.Select(q => q.Message.GetType().FullName))}]" },
            //         { "messaging.operation.name", "evaluate" },
            //         { "messaging.operation.type", "process" },
            //         { "messaging.destination.id", operation.aggregateId },
            //         { "messaging.destination.name", operation.aggregateType.FullName }
            //     });
            
            if (operation.aggregateType == null)
                throw new Exception($"Aggregate type could not be resolved for command batch");

            var handler = _handlerFactory.Instantiate(
                operation.aggregateType,
                operation.aggregateId);

            results.Add(await handler.Evaluate(operation.commands));
        }

        var result = Result
            .Ok(Array.Empty<IEventEnvelope>())
            .WithReasons(results.SelectMany(q => q.Reasons));
        
        if (result.IsSuccess)
            result.WithValue(results
                .SelectMany(q => q.ValueOrDefault != null
                    ? q.Value
                    : [])
                .ToArray());

        return result;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventEnvelopes"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<IResultBase> Apply(params IEventEnvelope[] eventEnvelopes)
    {
        // group commands by aggregate type and id to batch operations
        var eventCollections = eventEnvelopes
            .GroupBy(q => (
                aggregateType: _domainContext.EventHandlers
                    .Single(w => w.EventType == q.Message.GetType())
                    .AggregateType,
                aggregateId: q.Metadata.AggregateId
            ))
            .Select(q => (
                q.Key.aggregateType,
                q.Key.aggregateId,
                events: q.Select(w => w).ToArray(),
                result: new Result() as IResultBase
            ))
            .ToList();
        
        List<IResultBase> results = new(eventCollections.Count);
        
        foreach (var operation in eventCollections)
        {
            using var activity = DomainContext.ActivitySource.StartActivity(
                ActivityKind.Internal,
                name: $"apply {operation.aggregateType!.Name}",
                parentContext: _activity?.Context ?? default,
                tags: new Dictionary<string, object?>
                {
                    { "messaging.batch.message_count", operation.events.Length },
                    { "messaging.batch.types", $"[{string.Join(';', operation.events.Select(q => q.Message.GetType().FullName))}]" },
                    { "messaging.operation.name", "apply" },
                    { "messaging.operation.type", "settle" },
                    { "messaging.destination.id", operation.aggregateId },
                    { "messaging.destination.name", operation.aggregateType.FullName}
                });
            
            if (operation.aggregateType == null) throw new Exception($"Aggregate type could not be resolved for event {operation.aggregateType!.FullName}");

            var handler = _handlerFactory.Instantiate(
                operation.aggregateType,
                operation.aggregateId);

            results.Add(await handler.Apply(operation.events));
        }
        
        var result = new Result()
            .WithReasons(results.SelectMany(q => q.Reasons));
        
        return result;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventEnvelope"></param>
    /// <returns></returns>
    public Task<IResultBase> Continue(params IEventEnvelope[] eventEnvelopes)
    {
        foreach (var eventEnvelope in eventEnvelopes)
        {
            var sagas = _domainContext.Sagas
                .Where(q => q.EventType == eventEnvelope.Message.GetType())
                .Select(q => (ISaga)_services.GetRequiredService(q.HandlerType));
            
            foreach (var saga in sagas)
            {
                _ = Task.Run(() => Invoke(saga, eventEnvelope));
            }
        }
        
        return Task.FromResult<IResultBase>(Result.Ok());
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="saga"></param>
    /// <param name="eventEnvelope"></param>
    /// <returns></returns>
    public async Task<IResultBase> Invoke(ISaga saga, IEventEnvelope eventEnvelope)
    {
        using var activity = DomainContext.ActivitySource.StartActivity(
            ActivityKind.Internal,
            name: $"process {saga.GetType().Name}",
            links: [new ActivityLink()],
            tags: new Dictionary<string, object?>
            {
                { "messaging.message.type", eventEnvelope.Message.GetType().FullName },
                { "messaging.operation.name", "continue" },
                { "messaging.operation.type", "process" },
                { "messaging.destination.name", saga.GetType().FullName }
            });
        
        var context = new SagaContext(_services)
        {
            AggregateId = eventEnvelope.Metadata.AggregateId,
            ParentContext = activity?.Context
        };

        var result = await saga.Evaluate(context, eventEnvelope.Message);

        if (result.IsFailed) activity?.SetTag("error.type", "domain");
        
        return result;
    }

    public void Dispose()
    {
        _activity?.Dispose();
    }
}
