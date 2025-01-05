using FluentResults;

namespace Whaally.Domain.Abstractions;

public interface IServiceHandler : IMessageHandler
{
    public IAsyncEnumerable<IReason> Handle<TService>(IServiceHandlerContext context, TService service)
        where TService : class, IService;
}

public interface IServiceHandler<TService> : IServiceHandler
    where TService : class, IService
{
    IAsyncEnumerable<IReason> IServiceHandler.Handle<T>(IServiceHandlerContext context, T service)
        => Handle(context, (service as TService)!);

    public IAsyncEnumerable<IReason> Handle(IServiceHandlerContext context, TService service);
}
