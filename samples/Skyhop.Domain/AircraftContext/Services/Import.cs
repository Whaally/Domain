using FluentResults;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.AircraftContext.Services;

public record Import() : IService;

public class ImportHandler : IServiceHandler<Import>
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public async IAsyncEnumerable<IReason> Handle(IServiceHandlerContext context, Import service)
    {
        // Import things from some external service
        // We might use this construct to send progress updates on long running tasks
        yield break;
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}