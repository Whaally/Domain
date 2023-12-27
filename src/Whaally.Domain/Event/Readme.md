# Event
The event is a trace indicating that something had happened in the past. It is the source of truth, and used in event-sourced systems to derive the present state from. Events allow one to decouple the representation of state from the persistence of state.

Within this system the event caries the same semantic meaning as it does in an event-sourced system. The event results from the successful evaluation of a [command](../Command/Readme.md). Additionally an event is the sole component able to mutate the aggregates state.

> It is not strictly necessary to use event-sourcing techniques when using this library. However, the usage of commands and events are a nice abstraction to manage and compose change.

## Usage
Just like the [command](../Command/Readme.md) and [service](../Service/Readme.md), the event consist of two parts:

- `Event`: The DTO containing serializeable properties of the operation
- `EventHandler`: The object containing behaviour of the event

An event can be as simple as this:

```csharp
public record Created() : IEvent;
```

The event handler has to implement the `IEventHandler<TAggregate, TEvent>` interface, and looks like this:

```csharp
public class CreatedHandler : IEventHandler<Flight, Created>
{
    public Flight Apply(IEventHandlerContext<Flight> context, Created @event)
        => context.Aggregate with
        {
            IsInitialized = true
        };
}
```

The `Apply` method on the handler must return an instance of the aggregate, which replaces the current state of the aggregate with whatever the event handler returns.