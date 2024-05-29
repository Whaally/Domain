namespace Whaally.Domain.Abstractions.Aggregate;

public interface ISnapshot { }

public interface ISnapshot<TAggregate> : ISnapshot
    where TAggregate : class, IAggregate
{

}

public interface ISnapshotFactory<TAggregate, TSnapshot>
    where TSnapshot : ISnapshot
    where TAggregate : class, IAggregate
{
    public TSnapshot Instantiate(TAggregate aggregate);
}