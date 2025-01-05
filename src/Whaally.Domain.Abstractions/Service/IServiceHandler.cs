using FluentResults;

namespace Whaally.Domain.Abstractions;

public interface IServiceHandler : IMessageHandler
{
    public IAsyncEnumerable<IReason> Invoke<TService>(IServiceHandlerContext context, TService service)
        where TService : class, IService;
}

public interface IServiceHandler<TService> : IServiceHandler
    where TService : class, IService
{
    IAsyncEnumerable<IReason> IServiceHandler.Invoke<T>(IServiceHandlerContext context, T service)
        => Invoke(context, (service as TService)!);

    public IAsyncEnumerable<IReason> Invoke(IServiceHandlerContext context, TService service);
}
