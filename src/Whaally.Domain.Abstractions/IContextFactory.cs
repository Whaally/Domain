using System.Diagnostics;

namespace Whaally.Domain.Abstractions;

public interface IContextFactory
{
    /// <summary>
    ///     Create a new saga context from the provided event metadata
    /// </summary>
    /// <param name="metadata">Metadata of the event to continue</param>
    /// <param name="activity">Optional activity to share with the context</param>
    /// <returns>A new - single use - saga context</returns>
    public ISagaContext CreateSagaContext(
        EventMetadata metadata,
        Activity? activity = null);

    /// <summary>
    ///     Create a new service handler context for the provided service metadata
    /// </summary>
    /// <param name="metadata">Metadata of the service to create the context for</param>
    /// <param name="activity">Optional activity to share with this context instance</param>
    /// <returns>A new - single use - service handler context</returns>
    public IServiceHandlerContext CreateServiceHandlerContext(
        ServiceMetadata metadata,
        Activity? activity = null);

    /// <summary>
    ///     Create a new command handler context for the provided aggregate and command metadata
    /// </summary>
    /// <param name="aggregate">The aggregate to supply to this context</param>
    /// <param name="metadata">The metadata to supply to this context</param>
    /// <param name="activity">Optional activity to share with this context instance</param>
    /// <typeparam name="TAggregate">Type of the aggregate</typeparam>
    /// <returns>A new - single use - command handler context</returns>
    public ICommandHandlerContext<TAggregate> CreateCommandHandlerContext<TAggregate>(
        TAggregate aggregate,
        CommandMetadata metadata,
        Activity? activity = null)
        where TAggregate : class, IAggregate;

    /// <summary>
    ///     Create a new event handler context for the provided aggregate and event metadata
    /// </summary>
    /// <param name="aggregate">The aggregate to supply to this context</param>
    /// <param name="metadata">The metadata to supply to this context</param>
    /// <param name="activity">Optional activity to share with this context instance</param>
    /// <typeparam name="TAggregate">Type of the aggregate</typeparam>
    /// <returns>A new - single use - event handler context</returns>
    public IEventHandlerContext<TAggregate> CreateEventHandlerContext<TAggregate>(
        TAggregate aggregate, 
        EventMetadata metadata,
        Activity? activity = null)
        where TAggregate : class, IAggregate;
}
