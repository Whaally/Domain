using FluentResults;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Testing;

public abstract class ServiceTest<TService> : DomainTest
    where TService : class, IService
{
    public IServiceHandler<TService> Handler { get; } 
    public TService Service { get; }
    
    public ServiceHandlerContext Context { get; }
    public Result Result { get; private init; }
    public IEnumerable<ICommand> Commands { get; private init; }
    
    public ServiceTest(
        IServiceHandler<TService> handler,
        TService service)
    {
        Handler = handler;
        Service = service;
        
        var id = Guid.NewGuid();
        Context = new ServiceHandlerContext(Services, new ServiceMetadata())
        {
            ParentContext = default
        };

        // TODO: Reconsider this approach and refactor into an IAsyncLifetime structure
        var task = Handler.Invoke(Context, Service);

        if (task is {
            IsCompleted: false, 
            IsCanceled: false, 
            IsFaulted: false
        }) task.RunSynchronously();
        
        Result = Context.Result;
        Commands = Context.Commands.SelectMany(q => q.Messages);
    }
}
