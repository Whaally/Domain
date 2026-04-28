#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using Whaally.Domain.Abstractions;

namespace ConsoleSample.Domain;

public record NestedService : IService;

public class NestedServiceHandler : IServiceHandler<NestedService> {
    public async Task<IServiceResult> Invoke(IServiceHandlerContext context, NestedService service)
    {
        throw new NotImplementedException();
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
