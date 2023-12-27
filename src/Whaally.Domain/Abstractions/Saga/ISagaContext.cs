using FluentResults;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Service;

namespace Whaally.Domain.Abstractions.Saga;

public interface ISagaContext : IContext
{
    /// <summary>
    ///     The optimistic result of the evaluation of this saga.
    /// </summary>
    public IReadOnlyCollection<ICommandEnvelope> Commands { get; }

    /// <summary>
    ///     Stages a command as the optimistic result of this saga.
    /// </summary>
    /// <param name="command">The command staged as a result of saga evaluation</param>
    public void StageCommand(string aggregateId, ICommand command);

    /// <summary>
    ///     Evaluates a service and stages the resulting commands as the optimistic result of this saga.
    /// </summary>
    /// <param name="service">The service to evaluate as part of the saga evaluation</param>
    /// <returns>A Result object indicating success or error states.</returns>
    public Task<IResultBase> EvaluateService(IService service);

    /// <summary>
    ///     The AggregateHandlerFactory provides access to AggregateHandler instances.
    /// </summary>
    public IAggregateHandlerFactory Factory { get; }

    public string? AggregateId { get; }
}
