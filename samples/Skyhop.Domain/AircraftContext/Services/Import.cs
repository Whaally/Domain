using FluentResults;
using Whaally.Domain.Abstractions.Service;

namespace Skyhop.Domain.AircraftContext.Services;

public record Import() : IService;

public class ImportHandler : IServiceHandler<Import>
{
    public async Task<IResultBase> Handle(IServiceHandlerContext context, Import service)
    {
        // Import things from some external service
        
        return Result.Ok();
    }
}