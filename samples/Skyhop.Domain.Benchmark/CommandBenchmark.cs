using BenchmarkDotNet.Attributes;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Command;

namespace Skyhop.Domain.Benchmark;

public abstract class CommandBenchmark<TAggregate, TCommand>
    where TAggregate : class, IAggregate, new()
    where TCommand : class, ICommand
{
    public ICommandHandler<TAggregate, TCommand> Handler { get; } 
    public TAggregate Aggregate { get; }
    public TCommand Command { get; }
    
    public CommandHandlerContext<TAggregate> Context { get; }

    public CommandBenchmark(
        ICommandHandler<TAggregate, TCommand> handler, 
        TAggregate aggregate, 
        TCommand command)
    {
        Handler = handler;
        Aggregate = aggregate;
        Command = command;
        
        var id = Guid.NewGuid();
        Context = new CommandHandlerContext<TAggregate>(id.ToString())
        {
            Activity = default,
            Aggregate = Aggregate
        };
    }

    [Benchmark]
    public void Evaluate() => Handler.Evaluate(Context, Command);
}