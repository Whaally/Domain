using FluentResults;

namespace Whaally.Domain.Abstractions.Service;

public interface IServiceHandler : IMessageHandler
{
    public Task<IResultBase> Handle<TService>(IServiceHandlerContext context, TService service)
        where TService : class, IService;
}

public interface IServiceHandler<TService> : IServiceHandler
    where TService : class, IService
{
    Task<IResultBase> IServiceHandler.Handle<T>(IServiceHandlerContext context, T service)
        => Handle(context, (service as TService)!);

    public Task<IResultBase> Handle(IServiceHandlerContext context, TService service);
}