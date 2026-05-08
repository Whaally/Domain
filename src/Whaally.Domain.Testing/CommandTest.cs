using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Testing;

public abstract class CommandTest<TAggregate, TCommand> : DomainTest
    where TAggregate : class, IAggregate
    where TCommand : class, ICommand
{
    public ICommandHandler Handler { get; } 
    public TAggregate Aggregate { get; }
    public TCommand Command { get; }
    
    public CommandHandlerContext<TAggregate> Context { get; }
    public IResult Result { get; private init; }
    public IEnumerable<IEvent> Events { get; private init; }
    
    public CommandTest(
        TAggregate aggregate,
        TCommand command)
    {
        Handler = Domain.GetCommandHandler(typeof(TCommand));
        Aggregate = aggregate;
        Command = command;
        
        var id = Guid.NewGuid();
        
        // ToDo: Fix this usage
        Context = new CommandHandlerContext<TAggregate>(Services, id)
        {
            ParentContext = default,
            Aggregate = Aggregate
        };
        
        var task = Handler.Evaluate(Context, Command);

        // todo: figure out how to properly deal with async lifetime here
        if (task is {
            IsCompleted: false, 
            IsCanceled: false, 
            IsFaulted: false
        }) task.RunSynchronously();
        
        Result = task.Result;
        Events = task.Result.Operations
            .Cast<EventEnvelope>()
            .SelectMany(q => q.Messages);
    }
}
