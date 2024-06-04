using FluentAssertions;
using FluentResults;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;
using Whaally.Domain.Abstractions.Saga;
using Whaally.Domain.Event;
using Whaally.Domain.Saga;

namespace Whaally.Domain.Tests;

// ToDo: Test whether the multiple evaluation of a saga does not meaningfully change the state

public abstract class SagaTest<TEvent> : DomainTest
    where TEvent : class, IEvent
{
    public ISaga<TEvent> Saga { get; }
    public TEvent Event { get; }
    
    public SagaContext Context { get; }
    public Result Result { get; }
    public IEnumerable<ICommand> Commands { get; }

    public SagaTest(
        ISaga<TEvent> saga,
        TEvent @event)
        : this(default, saga, new EventEnvelope<TEvent>(@event, new EventMetadata(Guid.NewGuid().ToString()))) { }
    
    public SagaTest(
        Action<DomainContext>? initializer,
        ISaga<TEvent> saga,
        TEvent @event) : this(initializer, saga, new EventEnvelope<TEvent>(@event, new EventMetadata(Guid.NewGuid().ToString()))) { }
    
    public SagaTest(
        ISaga<TEvent> saga,
        IEventEnvelope<TEvent> @event) : this(default, saga, @event) { }
    
    public SagaTest(
        Action<DomainContext>? initializer,
        ISaga<TEvent> saga,
        IEventEnvelope<TEvent> @event) : base(initializer)
    {
        Saga = saga;
        Event = @event.Message;

        Context = new SagaContext(Services)
        {
            AggregateId = @event.Metadata.AggregateId,
            Activity = default
        };

        var output = Saga.Evaluate(Context, Event).Result;

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
