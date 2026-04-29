using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Generators;

namespace Whaally.Domain.Tests.SourceGenerators;

public class SagaMetadataGeneratorTest
{
    [Fact(Skip = "Used for manually triggering the source generator")]
    public async Task CanGenerateMetadata()
    {
        var test = new CSharpSourceGeneratorTest<GeneralizedMetadataGenerator, DefaultVerifier>
        {
            TestState =
            {
                Sources =
                {
                    ("TestInput.cs", 
                        """
                        using FluentResults;
                        using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;
                        using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
                        using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
                        using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Snapshots;
                        using Whaally.Domain;
                        using Whaally.Domain.Abstractions;
                        using Whaally.Domain.Generators;
                        
                        namespace Skyhop.Domain.AircraftContext.Sagas;
                        
                        [GenerateMetadata]
                        internal partial class OnArrival : ISaga<ArrivalTimeSet>
                        {
                            public async Task<ISagaResult> Evaluate(ISagaContext context, ArrivalTimeSet @event)
                            {
                                var snapshot = await context.Factory
                                    .Instantiate<Flight>(context.AggregateId!)
                                    .Snapshot<FlightSnapshot>();
                        
                                if (snapshot.AircraftId is Guid g)
                                    new Saga().Stage(
                                        g,
                                        new SetFlightInfo(
                                            g,
                                            snapshot.DepartureTime,
                                            @event.ArrivalTime));
                        
                                return new Saga();
                            }
                        }
                        """),
                    ("SetFlightInfo.cs",
                        """
                        using FluentResults;
                        using Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Events;
                        using Whaally.Domain;
                        using Whaally.Domain.Abstractions;
                        using Whaally.Domain.Generators;
                        
                        namespace Skyhop.Domain.AircraftContext.Aggregates.AircraftAggregate.Commands;
                        
                        [Immutable]
                        [GenerateSerializer]
                        public record SetFlightInfo(
                            Guid FlightId,
                            DateTime? Departure,
                            DateTime? Arrival) : ICommand;
                        
                        [GenerateMetadata]
                        public partial class SetFlightInfoHandler : ICommandHandler<Aircraft, SetFlightInfo>
                        {
                            public ICommandResult Evaluate(ICommandHandlerContext<Aircraft> context, SetFlightInfo command)
                            {
                                return new Command().Stage(
                                    new FlightInfoSet(command.FlightId, command.Departure, command.Arrival));
                            }
                        }
                        """),
                    ("Saga.cs",
                        """
                        using Whaally.Domain.Abstractions;
                        
                        namespace Whaally.Domain;
                        
                        public class Saga : Result<Saga>, ISagaResult
                        {
                            private List<IMessageEnvelope> _operations = new();
                            public IEnumerable<IMessageEnvelope> Operations => _operations.AsReadOnly();
                        
                            public ISagaResult Invoke(IService service)
                            {
                                _operations.Add(
                                    new ServiceEnvelope(
                                        new ServiceMetadata(),
                                        service));
                                
                                return this;
                            }
                        
                            public ISagaResult Stage(Guid aggregateId, params ICommand[] command)
                            {
                                _operations.Add(
                                    new CommandEnvelope(
                                        new CommandMetadata()
                                        {
                                            AggregateId = aggregateId
                                        }
                                        , command));
                                
                                return this;
                            }
                        }
                        
                        """)
                },
                GeneratedSources =
                {

                },
                AdditionalReferences =
                {
                    typeof(ICommandResult).Assembly,
                    typeof(Command).Assembly,
                    typeof(DepartureAirfieldSet).Assembly,
                    MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location),
                }
            }
        };

        await test.RunAsync();
    }
}