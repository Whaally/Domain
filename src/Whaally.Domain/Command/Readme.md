# Commands
A command is part of the [aggregate](../Aggregate/Readme.md), and is used to invoke a state change. It is the first part necessary to make a change. If success the command will return an [event](../Event/Readme.md) indicating how to change the aggregate involved.

In this library the concept of a command consists of two distinct parts:

- `Command`: The serializeable DTO containing information about what to do
- `CommandHandler`: The class containing the behaviour necessary to evaluate provided command

## Usage
The implementation of a command starts with the definition of the information which needs to be contained by the aggregate:

```csharp
public record Create() : ICommand;
```

The second part of the command is the handler, which is responsible for two things:

1. Evaluate whether the requested change is valid and may go ahead
2. To return [events](../Event/Readme.md) reflecting the intended changes

A command handler is defined like this:

```csharp
public class CreateHandler : ICommandHandler<Flight, Create>
{
    public IResultBase Evaluate(
        ICommandHandlerContext<Flight> context, 
        Create command)
    {
        context.StageEvent(new Created());

        return Result.Ok();
    }
}
```

The command has the authority to fail the operation. Rather than using exceptions, results are returned giving a more detailed insight into the failure mode.

> At the time of writing no explicit decision had been made about how to deal with events when a command fails. There's nothing preventing one from staging events and later failing the command. Are any staged events to be evaluated? Are they to be ignored? This is an area which might need further clarification later on.

## Design constraints
With the responsibilities of the command being well-defined, we can construct further compositions of more complex behaviour which are based upon these commands. For this to work it is required that commands are deterministic and free of side effects.

For this reason the command handler is a synchronous method, and should not be taking any dependencies beyond the aggregate instance (accessed through the context), and the provided event.

These constraints will grow more important once we start composing operations through [services](../Service/Readme.md).