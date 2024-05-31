using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Aggregate;
using Whaally.Domain.Event;

namespace Skyhop.Domain.Benchmark.Aggregate;

[ShortRunJob]
public class ApplyEventBenchmark
{
    public class Aggregate : IAggregate { }
    public class Event : IEvent { }

    public class EventHandler : IEventHandler<Aggregate, Event>
    {
        public Aggregate Apply(IEventHandlerContext<Aggregate> context, Event @event)
            => context.Aggregate;
    }

    private IServiceProvider _services;
    private DefaultAggregateHandler<Aggregate>? _handler;
    
    public ApplyEventBenchmark()
    {
        _services = new ServiceCollection()
            .AddSingleton<IAggregateHandlerFactory, DefaultAggregateHandlerFactory>()
            .AddTransient<IEvaluationAgent, DefaultEvaluationAgent>()
            .AddTransient<IEventHandler<Aggregate, Event>, EventHandler>()
            .BuildServiceProvider();
    }
    
    [GlobalSetup]
    public void Setup()
    {
        _handler = new DefaultAggregateHandler<Aggregate>(_services, "");
    }

    [Benchmark]
    public async Task Apply() => await _handler!.Apply(new EventEnvelope(new Event(), new EventMetadata("")));
}