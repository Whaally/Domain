using FluentResults;

namespace Whaally.Domain.Abstractions;

public interface ISagaContext : IContext
{
    /// <summary>
    ///     The AggregateHandlerFactory provides access to AggregateHandler instances.
    /// </summary>
    public IAggregateHandlerFactory Factory { get; }

    public string? AggregateId { get; }
}
