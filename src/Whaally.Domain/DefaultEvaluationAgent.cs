using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class DefaultEvaluationAgent : IEvaluationAgent
{
    private readonly IServiceProvider _services;
    private readonly DomainContext _domainContext;
    private readonly IAggregateHandlerFactory _handlerFactory;
    
    public DefaultEvaluationAgent(IServiceProvider services)
    {
        _services = services;
        _domainContext = _services.GetRequiredService<DomainContext>();
        _handlerFactory = _services.GetRequiredService<IAggregateHandlerFactory>();
    }
    
    private static void CheckSourceActivity(IMessageEnvelope[] envelopes)
    {
        if (envelopes.DistinctBy(q => q.Metadata.SourceActivity).Count() > 1)
            throw new Exception("All messages must originate from the same ActivityContext to evaluate them together");
    }
    
    public async Task<IResult<ICommandEnvelope[]>> Run<TService>(IServiceEnvelope<TService> serviceEnvelope)
        where TService : class, IService
    {
        var serviceHandler = (IServiceHandler<TService>)_services.GetRequiredService(_domainContext.ServiceHandlers
            .Single(q => q.ServiceType == typeof(TService))
            .HandlerType);
        
        var serviceHandlerContext = _services.GetRequiredService<IServiceHandlerContext>();

        var result = await serviceHandler.Handle<TService>(
            serviceHandlerContext,
            serviceEnvelope.Message);

        if (result.IsFailed) return Result.Fail<ICommandEnvelope[]>(result.Errors);

        return Result
            .Ok(new ICommandEnvelope[] { })
            .WithReasons(result.Reasons)
            .WithValue(serviceHandlerContext.Commands.ToArray());
    }
    
    public async Task<IResult<IEventEnvelope[]>> Evaluate(params ICommandEnvelope[] commandEnvelopes)
    {
        CheckSourceActivity(commandEnvelopes);

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
            if (operation.aggregateType == null) throw new Exception($"Aggregate type could not be resolved for command batch");

            var handler = _handlerFactory.Instantiate(
                operation.aggregateType,
                operation.aggregateId);

            results.Add(await handler.Evaluate(operation.commands));
        }

        var result = Result
            .Ok(new IEventEnvelope[] { })
            .WithReasons(results.SelectMany(q => q.Reasons));
        
        if (result.IsSuccess)
            result.WithValue(results
                .SelectMany(q => q.ValueOrDefault != null
                    ? q.Value
                    : [])
                .ToArray());

        return result;
    }

    public async Task<IResultBase> Apply(params IEventEnvelope[] eventEnvelopes)
    {
        CheckSourceActivity(eventEnvelopes);

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
            if (operation.aggregateType == null) throw new Exception($"Aggregate type could not be resolved for event {operation.aggregateType!.FullName}");

            var handler = _handlerFactory.Instantiate(
                operation.aggregateType,
                operation.aggregateId);

            results.Add(await handler.Apply(operation.events));
        }

        return Result
            .Ok()
            .WithReasons(results.SelectMany(q => q.Reasons));
    }
    
    public async Task<IResultBase> Continue(IEventEnvelope eventEnvelope)
    {
       /*
        * 1. Retrieve all relevant sagas
        * 2. Evaluate all relevant sagas
        * 3. Return resulting commands
        */

        var sagas = _domainContext.Sagas
           .Where(q => q.EventType == eventEnvelope.Message.GetType())
           .Select(q => (ISaga)_services.GetRequiredService(q.HandlerType));
        
        List<IResultBase> results = new();

        foreach (var saga in sagas) 
            results.Add(await RunSaga(saga, eventEnvelope));
        
        return new Result()
            .WithReasons(results.SelectMany(q => q.Reasons));
    }
    
    private async Task<IResultBase> RunSaga(ISaga saga, IEventEnvelope eventEnvelope)
    {
        var context = new SagaContext(_services)
        {
            AggregateId = eventEnvelope.Metadata.AggregateId,
            Activity = eventEnvelope.Metadata.SourceActivity
                .Continue(saga.GetType().Name)
                .Context
        };
        
        var sagaResult = await saga.Evaluate(
            context, 
            eventEnvelope.Message);
        
        if (sagaResult.IsFailed)
            return sagaResult;
        
        var evaluation = await Evaluate(context.Commands.ToArray());
        if (evaluation.IsFailed)
            return evaluation;
        
        var application = await Apply(evaluation.Value);
        if (application.IsFailed)
            return application;

        foreach (var @event in evaluation.Value) 
            await Continue(@event);

        return Result.Ok();
    }
}
