using System.Collections.Concurrent;
using FluentResults;
using Nito.AsyncEx;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class TransactionalAggregateHandler<TAggregate> : DefaultAggregateHandler<TAggregate>
    where TAggregate : class, IAggregate
{
    public TransactionalAggregateHandler(IServiceProvider services, string id) : base(services, id)
    {
    }
    
    public override async Task<IResult<EventEnvelope>> Evaluate(CommandEnvelope commandEnvelope)
    {
        var @lock = await AcquireLock(commandEnvelope.Metadata);
        if (@lock == null)
            return await Task.FromResult<IResult<EventEnvelope>>(Result.Fail<EventEnvelope>("Could not acquire lock"));
        
        return await base.Evaluate(commandEnvelope, @lock.Value.cancellationToken);
    }
    
    public override async Task<IResultBase> Apply(EventEnvelope eventEnvelope)
    {
        var @lock = await AcquireLock(eventEnvelope.Metadata);
        if (@lock == null)
            return await Task.FromResult<IResultBase>(Result.Fail<IResultBase>("Could not acquire lock"));
        
        var result = await base.Apply(eventEnvelope, @lock.Value.cancellationToken);
        
        ReleaseLock(@lock.Value.transactionId);
        
        return result;
    }
    
    public override Task Abort(IMessageMetadata metadata)
    {
        if (!string.IsNullOrWhiteSpace(metadata.TransactionId))
            ReleaseLock(metadata.TransactionId);
        
        return base.Abort(metadata);
    }
    
    // TODO: Automatically release any lock after a set amount of time: the timeout period
    // When this happens trigger any cancellation tokens to abort ongoing operations (if any), release the lock and let the
    // next transaction continue. Any work in progress fails with a timeout result.
    
    private ConcurrentQueue<(string transactionId, CancellationTokenSource cancellationToken)> _lockingQueue = new();
    private AsyncManualResetEvent _waitHandler = new(false);
    
    private async Task<(string transactionId, CancellationToken cancellationToken)?> AcquireLock(IMessageMetadata metadata)
    {
        if (string.IsNullOrEmpty(metadata.TransactionId)) return null;
        
        if (!_lockingQueue.Any(q => q.transactionId == metadata.TransactionId))
            _lockingQueue.Enqueue((metadata.TransactionId, new CancellationTokenSource()));
        
        if (_lockingQueue.TryPeek(out var @lock)
            && @lock.transactionId == metadata.TransactionId)
        {
            return (@lock.transactionId, @lock.cancellationToken.Token);
        }
        
        // ToDo: configure a timeout
        await _waitHandler.WaitAsync();
        
        return await AcquireLock(metadata);
    }
    
    private void ReleaseLock(string transactionId)
    {
        if (_lockingQueue.TryPeek(out var @lock)
            && @lock.transactionId == transactionId)
            _lockingQueue.TryDequeue(out _);
        else
        {
            var res = _lockingQueue
                .SingleOrDefault(q => q.transactionId == transactionId);
            res.cancellationToken?.Cancel();
        }
        
        _waitHandler.Set();
        _waitHandler.Reset();
    }
}
