using Whaally.Domain.Abstractions.Aggregate;

namespace Skyhop.Domain.AzureFunctions;

/// <summary>
///     An abstraction still allowing a somewhat strongly typed interaction with the durable entities.
/// </summary>
/// <param name="Id">The ID of the aggregate instance</param>
/// <typeparam name="TAggregate">The aggregate type</typeparam>
public record AggregateIdentity<TAggregate>(Guid Id) : AggregateIdentity(typeof(TAggregate).Name, Id)
    where TAggregate : class, IAggregate;

/// <summary>
///     An object holding the information based on which the aggregate can be instantiated.
/// </summary>
/// <param name="Type">The type of the aggregate to instantiate</param>
/// <param name="Id">The id of the aggregate</param>
public record AggregateIdentity(
    string Type, 
    Guid Id);
