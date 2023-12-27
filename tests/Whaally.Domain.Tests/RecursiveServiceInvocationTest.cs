using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions.Service;
using Whaally.Domain.Tests.Domain;

namespace Whaally.Domain.Tests;

public class RecursiveServiceInvocationTest
{
    readonly IServiceProvider _services = DependencyContainer
        .Services
        .AddTransient<IServiceHandler, RecursiveServiceHandler>()
        .AddTransient<IServiceHandler<RecursiveService>, RecursiveServiceHandler>()
        .BuildServiceProvider();

    public class RecursiveService : IService
    {
        public int Depth { get; init; }
    }

    public class RecursiveServiceHandler : IServiceHandler<RecursiveService>
    {
        public async Task<IResultBase> Handle(IServiceHandlerContext context, RecursiveService service)
        {
            if (service.Depth == 0)
            {
                return Result.Ok();
            }
            else
            {
                var newService = new RecursiveService
                {
                    Depth = service.Depth - 1
                };

                return await context.EvaluateService(newService);
            }
        }
    }

    /*
     * Design note:
     * The serviceHandler is currently the point of invocation for the service. In a future
     * version we'd want to invocate the service as is, without worrying about the underlying
     * service handler. For this we'd probably implement a ServiceEvaluator object or something
     * similarly named. The gist of it is that we abstract the retrieval of the handler itself away.
     *
     * Domain.Evaluate<TService>(TService service);
     * Domain.Retrieve<TAggregate>(Guid id);
     * Domain.
     */

    [Fact]
    public async Task RecursiveServiceCanBeEvaluated()
    {
        var service = new RecursiveService
        {
            Depth = 10
        };

        var serviceHandler = _services.GetRequiredService<IServiceHandler<RecursiveService>>();
        var serviceHandlerContext = _services.GetRequiredService<IServiceHandlerContext>();

        var result = await serviceHandler.Handle(serviceHandlerContext, service);

        Assert.True(result.IsSuccess);
    }
}