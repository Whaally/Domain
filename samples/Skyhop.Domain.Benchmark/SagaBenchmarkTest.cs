using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Whaally.Domain;
using Whaally.Domain.Abstractions.Aggregate;

namespace Skyhop.Domain.Benchmark;

public class SagaBenchmarkTest
{
    string? _flightId;
    string? _firstAircraftId;
    string? _secondAircraftId;

    IServiceProvider _services = new ServiceCollection()
        .AddDomain()
        .BuildServiceProvider();

    IAggregateHandlerFactory _factory => _services.GetRequiredService<IAggregateHandlerFactory>();

    IAggregateHandler<Flight>? fH;
    IAggregateHandler<Aircraft>? a1H;
    IAggregateHandler<Aircraft>? a2H;

    [GlobalSetup]
    public async Task Setup()
    {
            _flightId = Guid.NewGuid().ToString();
            _firstAircraftId = Guid.NewGuid().ToString();
            _secondAircraftId = Guid.NewGuid().ToString();

            a1H = _factory.Instantiate<Aircraft>(_firstAircraftId);
            a2H = _factory.Instantiate<Aircraft>(_secondAircraftId);
            fH = _factory.Instantiate<Flight>(_flightId);

            var c1 = new SetAircraft(_firstAircraftId);

            await fH.Continue(
                (await fH.Evaluate(c1))
                .Value);
        }

    [Benchmark]
    public async Task Test()
    {
            var c2 = new SetAircraft(_secondAircraftId!);

            await fH!.Continue(
                (await fH.Evaluate(c2))
                .Value);
        }
}