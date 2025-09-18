using FluentResults;

namespace Whaally.Domain.Abstractions;

public interface IServiceHandlerContext : IContext, IDisposable
{
    /// <summary>
    ///     The AggregateHandlerFactory provides access to AggregateHandler instances.
    /// </summary>
    public IAggregateHandlerFactory Factory { get; }
}
