using FluentResults;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Abstractions.Saga;
using Whaally.Domain.Abstractions.Service;

namespace Whaally.Domain.Abstractions;

/// <summary>
/// Central component providing behaviour for the high-level interaction between different domain components.
/// </summary>
public interface IEvaluationAgent
{
    /// <summary>
    /// Applies provided events to their respective aggregates.
    /// 
    /// If successfull this operation has side effects against aggregates involved!
    /// </summary>
    /// <param name="eventEnvelopes">The event envelopes to apply</param>
    /// <returns>Result object indicating success status</returns>
    public Task<IResultBase> EvaluateEvents(params IEventEnvelope[] eventEnvelopes);

    /// <summary>
    /// Evaluates the provided commands against their respective aggregates
    /// </summary>
    /// <param name="commandEnvelopes">The command envelopes to evaluate</param>
    /// <returns>A result object indicating success status and resulting events</returns>
    public Task<IResult<IEventEnvelope[]>> EvaluateCommands(params ICommandEnvelope[] commandEnvelopes);

    /*
     * Commands and events can be evaluated in batches because they are staged for evaluation.
     *
     * Sagas and events should be evaluated one by one because:
     * - Sagas have the potential to branch out evaluation. Partitioning that by event source prevents a single point of failure halting all continuations of an operation.
     * - Services are immediately evaluated upon invocation, only for the resulting commands to be staged. This gives some wiggle room for initiating compensatory actions.
     */

    /// <summary>
    /// Evaluates the supplied services
    /// </summary>
    /// <typeparam name="TService">The service to evaluate</typeparam>
    /// <param name="serviceEnvelope"></param>
    /// <returns>Commands representing the intended side effects of the service</returns>
    public Task<IResult<ICommandEnvelope[]>> EvaluateService<TService>(IServiceEnvelope<TService> serviceEnvelope)
        where TService : class, IService;

    /// <summary>
    /// Evaluates the sagas relevant for the supplied event
    /// </summary>
    /// <typeparam name="TEvent">The event for which to evaluate sagas</typeparam>
    /// <param name="eventEnvelope">The event envelope for which to continue evaluation</param>
    /// <returns>The resulting commands representing the intended side effects of the saga</returns>
    public Task<IResult<ICommandEnvelope[]>> EvaluateSaga(IEventEnvelope eventEnvelope);
}