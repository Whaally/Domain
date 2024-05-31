using BenchmarkDotNet.Attributes;
using FluentResults;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Command;

namespace Skyhop.Domain.Benchmark.Command;

public class StageEventBenchmark
{
    public class Aggregate : IAggregate { }
    public class Event : IEvent { }

    private CommandHandlerContext<Aggregate>? _context;
    
    [GlobalSetup]
    public void Setup()
    {
        _context = new CommandHandlerContext<Aggregate>(Guid.NewGuid().ToString())
        {
            Activity = default,
            Aggregate = new Aggregate()
        };
    }
    
    [Benchmark]
    public void Test()
    {
        _context!.StageEvent(new Event());
    }
}