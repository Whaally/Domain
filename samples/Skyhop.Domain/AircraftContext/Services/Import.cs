using FluentResults;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
using Whaally.Domain.Analyzers;

namespace Skyhop.Domain.AircraftContext.Services;

public record Import() : IService;

[GenerateMetadata]
public partial class ImportHandler : IServiceHandler<Import>
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async Task<IServiceResult> Invoke(IServiceHandlerContext context, Import service)
    {
        // Import things from some external service
        return new Service();
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}