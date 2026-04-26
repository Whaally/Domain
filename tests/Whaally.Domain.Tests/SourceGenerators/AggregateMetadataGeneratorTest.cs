using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Generators;

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
                        using Whaally.Domain.Generators;
                        using System.ComponentModel.DataAnnotations;
                        
                        namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
                        
                        [GenerateMetadata]
                        public record Flight(
                            [Required]
                            string Number) : IAggregate;
                        """)
                },
                AdditionalReferences =
                {
                    typeof(IAggregate).Assembly,
                    typeof(Command).Assembly,
                    typeof(ValidationAttribute).Assembly,
                    MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location),
                }
            }
        };
        
        

        await test.RunAsync();
    }
}