using FluentResults;
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
    public Result Result { get; private init; }
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
        Context = new CommandHandlerContext<TAggregate>(Services, id.ToString())
        {
            ParentContext = default,
            Aggregate = Aggregate
        };
        
        var output = Handler.Evaluate(Context, Command);

        if (output.GetType().GenericTypeArguments.Any())
            Result = new Result().WithReasons(output);
        else
            Result = (Result)output;
        
        Events = Context.Events;
    }
}
