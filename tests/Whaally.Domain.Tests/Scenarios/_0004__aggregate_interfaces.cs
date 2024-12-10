using System.Diagnostics;
using FluentAssertions;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Scenarios;

public class _0004__aggregate_interfaces
{
    public interface ITestAggregate : IAggregate;
    
    public class TestImplementation : ITestAggregate;
    
    // Commands
    public class TestCommand : ICommand;
    
    public class TestCommandHandler : ICommandHandler<ITestAggregate, TestCommand>
    {
        public IResultBase Evaluate(ICommandHandlerContext<ITestAggregate> context, TestCommand command)
            => throw new NotImplementedException();
    }
    
    // Events
    public class TestEvent : IEvent;
    
    public class TestEventHandler : IEventHandler<ITestAggregate, TestEvent>
    {
        public ITestAggregate Apply(IEventHandlerContext<ITestAggregate> context, TestEvent @event)
            => throw new NotImplementedException();
    }
    
    // Aggregates
    public class TestAggregateHandler : IAggregateHandler<ITestAggregate>
    {
        public Task<IResult<IEventEnvelope>> Evaluate(ICommandEnvelope commandEnvelope) 
            => throw new NotImplementedException();
        public Task<IResultBase> Apply(IEventEnvelope eventEnvelope) 
            => throw new NotImplementedException();
        public Task<IResult<IEventEnvelope[]>> Evaluate(params ICommandEnvelope[] commands)
            => throw new NotImplementedException();
        public Task<IResultBase> Apply(params IEventEnvelope[] events)
            => throw new NotImplementedException();
        public Task Abort(ActivityContext context) 
            => throw new NotImplementedException();
        public Task<TSnapshot> Snapshot<TSnapshot>() where TSnapshot : ISnapshot
            => throw new NotImplementedException();
    }

    public class TestAggregateFactory : IAggregateFactory
    {
        public T Instantiate<T>()
            where T : class
            => typeof(T) switch
            {
                Type t when t == typeof(ITestAggregate) => new TestImplementation() as T,
                _ => null
            } ?? throw new NotImplementedException("Instantiation of requested type T is not supported");
    }

    [Fact]
    public void AggregateHandlerDealsWithInterface()
        => typeof(TestAggregateHandler).Should().BeAssignableTo<IAggregateHandler<ITestAggregate>>();
    
    [Fact]
    public void TestEventHandlerDealsWithInterface()
        => typeof(TestEventHandler).Should().BeAssignableTo<IEventHandler<ITestAggregate, TestEvent>>();
    
    [Fact]
    public void TestCommandHandlerHasInterface()
        => typeof(TestCommandHandler).Should().BeAssignableTo<ICommandHandler<ITestAggregate, TestCommand>>();

    [Fact]
    public void CanCreateNewCommandContext() 
        => new CommandHandlerContext<ITestAggregate>(
            new ServiceCollection()
                .AddDomain(options =>
                {
                    options.AggregateFactory = _ => new TestAggregateFactory();
                })
                .BuildServiceProvider(), "");

    [Fact]
    public void CanCreateNewEventContext()
        => new EventHandlerContext<ITestAggregate>("");
    
    [Fact]
    public void CanCreateNewDefaultAggregateHandler()
        => new DefaultAggregateHandler<ITestAggregate>(
            new ServiceCollection()
                .AddDomain(config =>
                {
                    config.AggregateFactory = _ => new TestAggregateFactory();
                })
                .BuildServiceProvider(), "");
}
