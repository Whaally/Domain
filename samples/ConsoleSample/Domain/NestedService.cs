using FluentResults;
using Whaally.Domain.Abstractions;

namespace ConsoleSample.Domain;

public record NestedService : IService;

public class NestedServiceHandler : IServiceHandler<NestedService> {
    public Task<IResultBase> Handle(IServiceHandlerContext context, NestedService service) => Task.FromResult<IResultBase>(Result.Ok());
}