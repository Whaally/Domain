using BenchmarkDotNet.Attributes;
using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;

namespace Skyhop.Domain.Benchmark.Aggregate;

public class EmptyAggregateTest
{
    public record Aggregate : IAggregate { }
    public record Command : ICommand { }

    public class CommandHandler : ICommandHandler<Aggregate, Command>
    {
        public IResultBase Evaluate(ICommandHandlerContext<Aggregate> context, Command command)
            => Result.Ok();
    }
    
    [ShortRunJob]
    public class EmptyAggregate()
        : CommandBenchmark<Aggregate, Command>(new CommandHandler(), new Aggregate(), new Command())
    {

    }
}