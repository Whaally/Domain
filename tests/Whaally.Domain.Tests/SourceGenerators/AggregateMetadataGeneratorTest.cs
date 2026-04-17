using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Analyzers.Generators;

namespace Whaally.Domain.Tests.SourceGenerators;

public class AggregateMetadataGeneratorTest
{
    [Fact]
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
                        using Whaally.Domain.Abstractions;
                        using Whaally.Domain.Analyzers;
                        
                        namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
                        
                        [GenerateMetadata]
                        public record Flight : IAggregate { }
                        """)
                },
                GeneratedSources =
                {

                },
                AdditionalReferences =
                {
                    typeof(IAggregate).Assembly,
                    typeof(Command).Assembly,
                    MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location),
                }
            }
        };

        await test.RunAsync();
    }
}