using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Whaally.Domain;

namespace Skyhop.Domain.Benchmark.Aggregate;

[ShortRunJob]
public class DefaultAggregateHandlerFactoryBenchmark
{
    private IServiceProvider? _services;
    private Whaally.Domain.Domain? _domain;
    
    [GlobalSetup]
    public void Setup()
    {
        _services = new ServiceCollection()
            .AddDomain()
            .BuildServiceProvider();

        _domain = _services.GetRequiredService<Whaally.Domain.Domain>();
    }

    // 1.185 us
    [Benchmark]
    public void GetAggregate() => _domain!.GetAggregate<Flight>(Guid.NewGuid().ToString());

    [Benchmark]
    public void EvaluateCommand() => _ = _domain!.EvaluateCommand(Guid.NewGuid().ToString(), new Create());

    [Benchmark]
    public async Task GetAggregateThenEvaluateCommand()
    {
        var aggregate = await _domain!.GetAggregate<Flight>(Guid.NewGuid().ToString());
        _ = await aggregate.Evaluate(new Create());
    }
}