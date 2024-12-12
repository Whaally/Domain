using System.Collections.Concurrent;
using FluentResults;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class TransactionalAggregateHandler<TAggregate> : DefaultAggregateHandler<TAggregate>
    where TAggregate : class, IAggregate
{
    
    
    public TransactionalAggregateHandler(IServiceProvider services, string id) : base(services, id)
    {
    }

    public override Task<IResult<IEventEnvelope>> Evaluate(ICommandEnvelope commandEnvelope)
    {
        if (!AcquireLock(commandEnvelope.Metadata))
            return Task.FromResult<IResult<IEventEnvelope>>(Result.Fail<IEventEnvelope>("Could not acquire lock"));
        
        return base.Evaluate(commandEnvelope);
    }

    public override Task<IResultBase> Apply(IEventEnvelope eventEnvelope)
    {
        if (!AcquireLock(eventEnvelope.Metadata))
            return Task.FromResult<IResultBase>(Result.Fail<IResultBase>("Could not acquire lock"));
        
        var result = base.Apply(eventEnvelope);

        ReleaseLock(eventEnvelope.Metadata);
        
        return result;
    }

    public override Task Abort(IMessageMetadata metadata)
    {
        ReleaseLock(metadata);
        return base.Abort(metadata);
    }

    // TODO: Automatically release any lock after a set amount of time: the timeout period
    // When this happens trigger any cancellation tokens to abort ongoing operations (if any), release the lock and let the
    // next transaction continue. Any work in progress fails with a timeout result.
    
    private ConcurrentQueue<string> _lockingQueue = new();
    private EventWaitHandle _waitHandler = new(false, EventResetMode.ManualReset);
    
    private bool AcquireLock(IMessageMetadata metadata)
    {
        if (string.IsNullOrEmpty(metadata.TransactionId)) return false;
        
        if (!_lockingQueue.Any(q => q == metadata.TransactionId))
            _lockingQueue.Enqueue(metadata.TransactionId);
        
        if (_lockingQueue.TryPeek(out var txId)
            && txId == metadata.TransactionId) return true;

        _waitHandler.WaitOne();
        
        return AcquireLock(metadata);
    }

    private void ReleaseLock(IMessageMetadata metadata)
    {
        if (_lockingQueue.TryPeek(out var txId)
            && txId == metadata.TransactionId)
            _lockingQueue.TryDequeue(out _);
        else
            _lockingQueue = new ConcurrentQueue<string>(_lockingQueue.Where(q => q != metadata.TransactionId));
        
        _waitHandler.Set();
        _waitHandler.Reset();
    }
}
