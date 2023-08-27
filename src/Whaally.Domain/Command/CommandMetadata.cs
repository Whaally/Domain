using System.Diagnostics;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Command
{
    public record CommandMetadata : ICommandMetadata
    {
        // There are valid reasons for why the AggregateId might not be set.
        // One of them is becasue the command is supplied to an AggregateHandler instance
        // thus already containing a reference to the aggregate.
        public string AggregateId { get; init; } = "";
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
        public ActivityContext SourceActivity { get; init; }
    }
}
