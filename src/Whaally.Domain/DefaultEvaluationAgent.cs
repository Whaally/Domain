using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Abstractions.Saga;
using Whaally.Domain.Abstractions.Service;
using Whaally.Domain.Saga;

namespace Whaally.Domain;

public class DefaultEvaluationAgent : IEvaluationAgent
{
    private readonly IServiceProvider _services;

    public DefaultEvaluationAgent(IServiceProvider services)
    {
        _services = services;
    }

    private static void CheckSourceActivity(IMessageEnvelope[] envelopes)
    {
        if (envelopes.DistinctBy(q => q.Metadata.SourceActivity).Count() > 1)
            throw new Exception("All messages must originate from the same ActivityContext to evaluate them together");
    }

    public async Task<IResult<IEventEnvelope[]>> EvaluateCommands(params ICommandEnvelope[] commandEnvelopes)
    {
        CheckSourceActivity(commandEnvelopes);

        var handlerFactory = _services.GetRequiredService<IAggregateHandlerFactory>();

        // group commands by aggregate type and id to batch operations
        var commandCollections = commandEnvelopes
            .GroupBy(q => (
                aggregateType: _services.GetRelatedAggregateTypeForOperation(q.Message.GetType()),
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

            var handler = handlerFactory.Instantiate(
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
                    : new IEventEnvelope[] { })
                .ToArray());

        return result;
    }

    public async Task<IResultBase> EvaluateEvents(params IEventEnvelope[] eventEnvelopes)
    {
        CheckSourceActivity(eventEnvelopes);

        var handlerFactory = _services.GetRequiredService<IAggregateHandlerFactory>();

        // group commands by aggregate type and id to batch operations
        var eventCollections = eventEnvelopes
            .GroupBy(q => (
                aggregateType: _services.GetRelatedAggregateTypeForOperation(q.Message.GetType()),
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

            var handler = handlerFactory.Instantiate(
                operation.aggregateType,
                operation.aggregateId);

            results.Add(await handler.Apply(operation.events));
        }

        return Result
            .Ok()
            .WithReasons(results.SelectMany(q => q.Reasons));
    }

    public async Task<IResult<ICommandEnvelope[]>> EvaluateSaga(IEventEnvelope eventEnvelope)
    {
       /*
        * 1. Retrieve all relevant sagas
        * 2. Evaluate all relevant sagas
        * 3. Return resulting commands
        */

        var sagas = _services.GetServices(
            typeof(ISaga<>)
                .MakeGenericType(eventEnvelope.Message.GetType()))
            .Cast<ISaga>();

        List<IResult<ICommandEnvelope[]>> results = new();

        foreach (var saga in sagas)
        {
            // There is no relevant saga registered. That's ok.
            if (saga == null) continue;

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

            var result = new Result<ICommandEnvelope[]>()
                .WithReasons(sagaResult.Reasons);

            if (sagaResult.IsSuccess)
                result.WithValue(context.Commands.ToArray());

            results.Add(result);
        }

        return new Result<ICommandEnvelope[]>()
            .WithValue(results
                .SelectMany(q => q.ValueOrDefault)
                .Where(q => q != null)
                .ToArray())
            .WithReasons(results.SelectMany(q => q.Reasons));
    }

    public async Task<IResult<ICommandEnvelope[]>> EvaluateService<TService>(IServiceEnvelope<TService> serviceEnvelope)
        where TService : class, IService
    {
        var serviceHandler = _services.GetRequiredService<IServiceHandler<TService>>();
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
}