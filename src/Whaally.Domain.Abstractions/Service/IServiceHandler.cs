namespace Whaally.Domain.Abstractions;

public interface IServiceHandler : IMessageHandler
{
    public Task Invoke<TService>(IServiceHandlerContext context, TService service)
        where TService : class, IService;
}

public interface IServiceHandler<TService> : IServiceHandler
    where TService : class, IService
{
    Task IServiceHandler.Invoke<T>(IServiceHandlerContext context, T service)
        => Invoke(context, (service as TService)!);

    public Task Invoke(IServiceHandlerContext context, TService service);
}
