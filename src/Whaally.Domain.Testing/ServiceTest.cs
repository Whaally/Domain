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
        var output = new Result().WithReasons(Handler.Invoke(Context, Service).ToBlockingEnumerable());
        
        if (output.GetType().GenericTypeArguments.Any())
            Result = new Result().WithReasons(output.Reasons);
        else
            Result = (Result)output;
        
        Commands = Context.Commands.SelectMany(q => q.Messages);
    }
}
