# Sagas
In software systems the saga is a curious component. If only for the different interpretations people have of the concept of a "saga". The common denominator is that sagas are considered to be long lived processes; that is that an arbitrary amount of time may be in between distinct steps of the process.

> For a deep dive into the saga pattern, check out [this slide deck by Uwe Friedrichsen: "Beyon the saga pattern"](https://speakerdeck.com/ufried/beyond-the-saga-pattern). 

## Usage
This is an example of the bare minimum for a saga:

```csharp
public class OnDeparture : ISaga<DepartureTimeSet>
{
    public async Task<IResultBase> Evaluate(
        ISagaContext context, 
        DepartureTimeSet @event)
    {
        // Do something...

        return Result.Ok();
    }
}
```

The saga (`ISaga<T>`) must contain the type of the event in response of which it should be executed.

Conceptually the saga is similar to a service. The difference between the two is that a service can be invoked from code, whereas a saga is executed in response to an event. The similarities are in their asynchronous evaluation, access to external resources and lack of identity.

## Testing
To test the correct evaluation of a saga tests can be constructed in different ways. The easiest is to change the parallel evaluation of sagas to sequential. While this is not recommended in production environments, it allows for easier testing, reasoning, and performance benchmarking in test environments.

```
// ToDo: Currently saga evaluation happens sequential. Evaluate in parallel by default, and allow modification of default behaviour for test cases.
```

```csharp
[Fact]
public async Task DepartureTimeSet_Should_Trigger_OnDeparture() {
    var aircraftId = Guid.NewGuid().ToString();
    var flightId = Guid.NewGuid().ToString();
    var departureAirfieldId = Guid.NewGuid().ToString();

    var flight = await AggregateFactory
        .Instantiate<Flight>(flightId)
        .EvaluateAndApply(
            new Create(),
            new SetAircraft(aircraftId),
            new SetDeparture(
                DateTime.UtcNow,
                departureAirfieldId
            ));
    
    var aircraft = await AggregateFactory
        .Instantiate<Aircraft>(aircraftId)
        .Snapshot<AircraftSnapshot>();
    
    Assert.Contains(flightId, aircraft.FlightsIds);
}
```

The previous test only indirectly asserts the correct functioning of the saga. There is nothing directly involving the saga. An alternative to test only the saga without anything else is shown here:

```csharp
[Fact]
public async Task OnDeparture_Should_Stage_SetFlightInfo()
{
    var flightId = Guid.NewGuid().ToString();
    
    // First we're instantiating a flight as a snapshot of it will be retrieved by the saga
    await AggregateFactory
        .Instantiate<Flight>(flightId)
        .EvaluateAndApply(
            new Create(),
            new SetAircraft(Guid.NewGuid().ToString()));
    
    // Then we're creating the saga, and instantiating the arguments required for evaluation
    var saga = new OnDeparture();
    
    var context = new SagaContext(Services)
    {
        AggregateId = flightId
    };
    var @event = new DepartureTimeSet(DateTime.Now);

    // Evaluate
    var result = await saga.Evaluate(context, @event);
    
    // Assert
    Assert.True(result.IsSuccess);
    Assert.Single(context.Commands);
    Assert.IsType<SetFlightInfo>(context.Commands.Single().Message);
}
```

It may be evident that such specific tests, dependent on the complexity of the saga, may become significantly more complex than just implicitly testing the visible side-effects. Additional the test solely testing the saga itself is not how domain interaction naturally happens. This is something to consider when organizing your tests, and when you want to use tests as living documentation on how to interact with the domain layer.

## Evaluation
To work nicely in a distributed execution environment, evaluation of the saga is at least once. This means that the same saga can - and will - be executed multiple times for the same resources.

To prevent the need for a data store to store saga evaluation state, sagas are evaluated at two distinct times:

1. Upon aggregate activation (for all events previously applied to the aggregate instance)
2. Upon event application (for the events being applied right now)

This provides flexibility to alter the behaviour of the application. This allows one to resolve evaluation failures at a later point in time. A more exotic use-case of sagas then is the evaluation of some behaviour based on an event that had been issued in the past. This latter case thus allows some flexibility in the evolution of a domain.

It should be noted that a saga itself is responsible for determining whether or not to incur any side-effects. There are no safeguards whatsoever barring one from repeatedly applying the same side-effects over and over again. This is the case throughout this whole library, though becomes particularly visible on the saga.

Saga evaluation is asynchronous. Therefore there is no guarantee that the aggregate is in a certain state whenever the saga is evaluated. Whenever the aggregate is activated for evaluation of a command, it is quite likely that the command is evaluated (and any resulting events applied) before sagas are evaluated. This is necessary to keep aggregate activation times low.
