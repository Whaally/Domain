using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Command;
using Whaally.Domain.Event;
using Whaally.Domain.Tests.Domain;

namespace Whaally.Domain.Tests
{
    public class TypeInferenceTests
    {
        [Fact]
        public void CommandHandler_Can_Be_Casted()
        {
            var handler = new TestCommandHandler();

            Assert.True(handler is ICommandHandler);
            Assert.True(handler is ICommandHandler<TestAggregate, TestCommand>);
            Assert.False(handler is ICommandHandler<IAggregate, ICommand>);
        }

        [Fact]
        public void EventHandler_Can_Be_Casted()
        {
            var handler = new TestEventHandler();

            Assert.True(handler is IEventHandler);
            Assert.True(handler is IEventHandler<TestAggregate, TestEvent>);
            Assert.False(handler is IEventHandler<IAggregate, IEvent>);
        }

        [Fact]
        public void CommandHandlerContext_Can_Be_Generalized()
        {
            var context = new CommandHandlerContext<TestAggregate>("")
            {
                Aggregate = new TestAggregate()
            };

            Assert.True(context is ICommandHandlerContext);
            Assert.True(context is ICommandHandlerContext<TestAggregate>);
        }

        [Fact]
        public void EventHandlerContext_Can_Be_Generalized()
        {
            var context = new EventHandlerContext<TestAggregate>("")
            {
                Aggregate = new TestAggregate()
            };

            Assert.True(context is IEventHandlerContext);
            Assert.True(context is IEventHandlerContext<TestAggregate>);
        }

        [Fact]
        public void Generic_EventEnvelope_Is_Generic_IEventEnvelope()
        {
            EventEnvelope<TestEvent> eventEnvelope = new(new(), new EventMetadata(""));

            Assert.True(eventEnvelope is IEventEnvelope);
            Assert.True(eventEnvelope is IEventEnvelope<TestEvent>);
        }

        [Fact]
        public void Generic_EventEnvelope_Is_Interface()
        {
            IEventEnvelope<IEvent> eventEnvelope = new EventEnvelope<TestEvent>(new(), new EventMetadata(""));

            Assert.True(eventEnvelope is EventEnvelope<TestEvent>);
            Assert.True(eventEnvelope is IEventEnvelope);
            Assert.True(eventEnvelope is IEventEnvelope<IEvent>);
        }
    }
}
