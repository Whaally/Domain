using Whaally.Domain.Abstractions.Aggregate;

namespace Whaally.Domain.Abstractions;

public interface IProvideAggregateInstance
{
    public IAggregate Aggregate { get; init; }
}

public interface IProvideAggregateInstance<TAggregate> : IProvideAggregateInstance
    where TAggregate : class, IAggregate
{
    public new TAggregate Aggregate { get; init;  }

    IAggregate IProvideAggregateInstance.Aggregate
    {
        get => Aggregate;
        init => Aggregate = (TAggregate)value;
    }
}