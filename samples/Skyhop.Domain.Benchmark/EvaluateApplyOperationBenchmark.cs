using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Whaally.Domain;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Aggregate;

namespace Skyhop.Domain.Benchmark
{
    public class EvaluateApplyOperationBenchmark
    {
        IServiceProvider _services = new ServiceCollection()
            .AddDomain()
            .BuildServiceProvider();

        IAggregateHandler<Flight>? _aggregateHandler;

        string _g = Guid.NewGuid().ToString();
        ICommand[] _c;
        
        public EvaluateApplyOperationBenchmark()
        {
            _c = new ICommand[] {
                new SetDeparture(_g, DateTime.UtcNow.AddHours(-1), Guid.NewGuid().ToString()),
                new SetArrival(_g, DateTime.UtcNow, Guid.NewGuid().ToString()),
                new SetAircraft(_g, Guid.NewGuid().ToString())
            };
        }

        [GlobalSetup]
        public void Prepare()
        {
            // ToDo: Ensure the aggregate is never null upon initialization!
            _aggregateHandler = new DefaultAggregateHandler<Flight>(_services, "")
            {
                Aggregate = new()
            };
        }

        [Benchmark]
        public async Task Run()
        {
            await _aggregateHandler!.Apply(
                (await _aggregateHandler.Evaluate(_c)).Value);
        }
    }
}
