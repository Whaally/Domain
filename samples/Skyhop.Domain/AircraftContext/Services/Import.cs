using FluentResults;
using Whaally.Domain.Abstractions;

namespace Skyhop.Domain.AircraftContext.Services;

public record Import() : IService;

public class ImportHandler : IServiceHandler<Import>
{
    public Task<IResultBase> Handle(IServiceHandlerContext context, Import service)
    {
        // Import things from some external service
        
        return Task.FromResult<IResultBase>(Result.Ok());
    }
}