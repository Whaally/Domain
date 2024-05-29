using FluentAssertions;
using FluentResults;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Service;
using Whaally.Domain.Service;

namespace Whaally.Domain.Tests;

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
        // ToDo: Provide IServiceProvider
        Context = new ServiceHandlerContext(default!)
        {
            Activity = default
        };
        
        var output = Handler.Handle(Context, Service).Result;
        
        if (output.GetType().GenericTypeArguments.Any())
            Result = new Result().WithReasons(output.Reasons);
        else
            Result = (Result)output;
        
        Commands = Context.Commands.Select(q => q.Message);
    }

    [Fact]
    public void UponFailureAssertNoCommands()
    {
        if (Result.IsFailed)
        {
            Commands.Should().BeEmpty();
        }
    }

    [Fact]
    public void AssertResultDoesNotContainObject()
    {
        Result.GetType().Should().NotBeOfType(typeof(IResult<>));
    }
}
