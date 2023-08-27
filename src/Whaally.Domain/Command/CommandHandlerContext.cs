using System.Diagnostics;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Event;

namespace Whaally.Domain.Command
{
    public class CommandHandlerContext<TAggregate> : ICommandHandlerContext<TAggregate>
        where TAggregate : class, IAggregate, new()
    {
        public CommandHandlerContext(string aggregateId)
        {
            AggregateId = aggregateId;
        }

        public IReadOnlyCollection<IEventEnvelope> Events => _events.AsReadOnly();
        private List<IEventEnvelope> _events = new List<IEventEnvelope>();

        public TAggregate Aggregate { get; init; } = new();
        public ActivityContext Activity { get; init; }
        public string AggregateId { get; init; }

        public void StageEvent<TEvent>(TEvent @event)
            where TEvent : class, IEvent
        {
            var envelope = new EventEnvelope<TEvent>(
                @event,
                new EventMetadata(AggregateId)
                {
                    SourceActivity = Activity
                });

            _events.Add(envelope);
        }
    }
}
