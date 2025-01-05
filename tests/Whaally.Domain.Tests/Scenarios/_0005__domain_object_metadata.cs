using FluentAssertions;
using FluentResults;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Tests.Scenarios;

/// <summary>
///     The goal of this test is to assert that domain metadata can be correctly stored and interacted with
/// </summary>
public class _0005__domain_object_metadata
{
    class TestAggregate : IAggregate;
    class TestCommand : ICommand;
    class TestEvent : IEvent;
    class TestService : IService;
    class TestSnapshot : ISnapshot<TestAggregate>;
    
    class TestCommandHandler : ICommandHandler
    {
        public IEnumerable<IReason> Evaluate(ICommandHandlerContext context, ICommand command) => throw new NotImplementedException();
    }

    class TestCommandHandlerWithGenerics : ICommandHandler<TestAggregate, TestCommand>
    {
        public IEnumerable<IReason> Evaluate(ICommandHandlerContext<TestAggregate> context, TestCommand command) => throw new NotImplementedException();
    }

    class TestEventHandler : IEventHandler
    {
        public TAggregate Apply<TAggregate>(IEventHandlerContext<TAggregate> context, IEvent @event) where TAggregate : class, IAggregate => throw new NotImplementedException();
    }
    
    class TestEventHandlerWithGenerics : IEventHandler<TestAggregate, TestEvent>
    {
        public TestAggregate Apply(IEventHandlerContext<TestAggregate> context, TestEvent @event) => throw new NotImplementedException();
    }
    
    class TestSaga : ISaga<TestEvent>
    {
        public Task<IResultBase> Evaluate(ISagaContext context, TestEvent @event) => throw new NotImplementedException();
    }
    
    class TestServiceHandler : IServiceHandler<TestService>
    {
        public Task<IResultBase> Handle(IServiceHandlerContext context, TestService service) => throw new NotImplementedException();
    }
    
    class TestSnapshotFactory : ISnapshotFactory<TestAggregate, TestSnapshot>
    {
        public TestSnapshot Instantiate(TestAggregate aggregate) => throw new NotImplementedException();
    }

    [Fact]
    public void CanGenerateMetadataFromCommandHandlerType()
    {
        var meta = CommandHandlerMeta.From<TestCommandHandler>();

        meta.HandlerType.Should().Be(typeof(TestCommandHandler));
        meta.AggregateType.Should().BeNull();
        meta.CommandType.Should().BeNull();
    }

    [Fact]
    public void CanGenerateMetadataFromCommandHandlerWithGenerics()
    {
        var meta = CommandHandlerMeta.From<TestCommandHandlerWithGenerics>();

        meta.HandlerType.Should().Be(typeof(TestCommandHandlerWithGenerics));
        meta.AggregateType.Should().Be(typeof(TestAggregate));
        meta.CommandType.Should().Be(typeof(TestCommand));
    }

    [Fact]
    public void Event_CanGenerateMetadata()
    {
        var meta = EventHandlerMeta.From<TestEventHandler>();

        meta.HandlerType.Should().Be(typeof(TestEventHandler));
        meta.AggregateType.Should().BeNull();
        meta.EventType.Should().BeNull();
    }

    [Fact]
    public void Event_CanGenerateMetadataForGeneric()
    {
        var meta = EventHandlerMeta.From<TestEventHandlerWithGenerics>();

        meta.HandlerType.Should().Be(typeof(TestEventHandlerWithGenerics));
        meta.AggregateType.Should().Be(typeof(TestAggregate));
        meta.EventType.Should().Be(typeof(TestEvent));
    }

    [Fact]
    public void Saga_CanGenerateMeta()
    {
        var meta = SagaMeta.From<TestSaga>();

        meta.HandlerType.Should().Be(typeof(TestSaga));
        meta.EventType.Should().Be(typeof(TestEvent));
    }

    [Fact]
    public void Service_CanGenerateMeta()
    {
        var meta = ServiceHandlerMeta.From<TestServiceHandler>();

        meta.HandlerType.Should().Be(typeof(TestServiceHandler));
        meta.ServiceType.Should().Be(typeof(TestService));
    }
    
    [Fact]
    public void Service_ThrowsIfNotService()
    {
        Assert.Throws<ArgumentException>(() => ServiceHandlerMeta.From(typeof(TestCommandHandler)));
    }
    
    [Fact]
    public void Service_ThrowsIfInterface()
    {
        Assert.Throws<ArgumentException>(() => ServiceHandlerMeta.From(typeof(IServiceHandler)));
    }

    [Fact]
    public void Snapshot_CanGenerateMeta()
    {
        var meta = SnapshotFactoryMeta.From<TestSnapshotFactory>();

        meta.FactoryType.Should().Be(typeof(TestSnapshotFactory));
        meta.AggregateType.Should().Be(typeof(TestAggregate));
        meta.SnapshotType.Should().Be(typeof(TestSnapshot));
    }
}
