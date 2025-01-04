using FluentResults;
using Orleans.Concurrency;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Infrastructure.OrleansHost;

public interface IAggregateHandlerGrain<TAggregate> : IGrainWithGuidKey,
    IAggregateHandler<TAggregate>
    where TAggregate : class, IAggregate
{
    [AlwaysInterleave]
    new Task<IResult<EventEnvelope>> Evaluate(CommandEnvelope commandEnvelope);

    [AlwaysInterleave]
    new Task Abort(IMessageMetadata metadata);

    [AlwaysInterleave]
    new Task<IResultBase> Apply(EventEnvelope eventEnvelope);

    Task IAggregateHandler.Abort(IMessageMetadata metadata)
    {
        return Abort(metadata);
    }

    Task<IResultBase> IAggregateHandler.Apply(EventEnvelope eventEnvelope)
    {
        return Apply(eventEnvelope);
    }

    Task<IResult<EventEnvelope>> IAggregateHandler.Evaluate(CommandEnvelope commandEnvelope)
    {
        return Evaluate(commandEnvelope);
    }
}
