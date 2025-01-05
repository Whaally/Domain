#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using FluentResults;
using Whaally.Domain.Abstractions;

namespace ConsoleSample.Domain;

public record NestedService : IService;

public class NestedServiceHandler : IServiceHandler<NestedService> {
    public async Task Invoke(IServiceHandlerContext context, NestedService service)
    {
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
