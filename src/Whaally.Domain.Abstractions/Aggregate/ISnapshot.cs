namespace Whaally.Domain.Abstractions;

public interface ISnapshot;

// ReSharper disable once UnusedTypeParameter
// ReSharper disable once UnusedType.Global
public interface ISnapshot<TAggregate> : ISnapshot
    where TAggregate : class, IAggregate;

/// <summary>
///     Marker interface to discover ISnapshotFactory<,> implementations.
/// </summary>
public interface ISnapshotFactory;

public interface ISnapshotFactory<TAggregate, TSnapshot> : ISnapshotFactory
    where TSnapshot : ISnapshot
    where TAggregate : class, IAggregate
{
    public TSnapshot Instantiate(TAggregate aggregate);
}
