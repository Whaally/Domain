# Services
In DDD terminology a service is a component running operations involving multiple aggregates, and which may invoke external services.

In this library the DDD-related meaning of a service is preserved. At the same time there are some expectations around the way services invoke change against the remainder of the system to maintain scalable characteristics. This results in a unique design where a service can only indicate the side-effects it wants as a result, but does not have the authority nor ability to persist these side effects on its own.

## Usage
In line with the [command](../Command/Readme.md) and [event](../Event/Readme.md), the service consists of both a serializeable DTO and a handler.

```csharp
public record Import() : IService;
```

The handler then provides the concrete behaviour:

```csharp
public class ImportHandler : IServiceHandler<Import>
{
    public async Task<IResultBase> Handle(IServiceHandlerContext context, Import service)
    {
        // Import things from some external service
        
        return Result.Ok();
    }
}
```

Different from the command and event is that the `IServiceHandler` only takes a single generic, being the `IService` DTO. The command and event handlers would need an additional generic being the aggregate which they are part of.

In interaction with the service handler, the context can be used to:

- Retrieve an aggregate handler instance for interaction with aggregates
- Stage a command
- Evaluate a service

Note that any commands ran through an aggregate handler instance are not part of standard service evaluation dynamics as discussed below.

## Evaluation semantics
A service is almost free to do whatever it wants, although the ways it may invoke side effects are rather limited. To do this it can do one of two things:

1. Stage commands
2. Evaluate other services

While commands will only be evaluated after the service had finished evaluating, other services are invoked on-demand. Internally this results in a list of commands in the same order in which they had been staged.

Further evaluation of the resulting commands can be parallelized over all aggregate instances involved. This execution model has the following benefits:

- Complex operations can be represented through a collection of commands
- Side effects may be applied only if all participating commands had successfully evaluated
- Application of events resulting from these commands can be parallelized over all participating aggregate instances

## Distributed transactions
The evaluation semantics of services makes them ideal for combination with a distributed commit protocol. Using a distributed commit protocol is ideal to coordinate multiple pieces of evaluation logic, and treat them together as a single unit of change. This helps to limit the amount of recovery logic we have to write when a single small scale operation fails.

It is expected that the failure rates for distributed commit protocols increase with the complexity of the distributed operation. The bigger and more complex the operation, the more external factors are involved which one does not have any control over.

What distributed commit protocols do not necessarily help with is the detection of infrastructural failures. If a service is unavailable, then how do we continue? In the worst case a distributed system fractures into a split-brain condition where both parts continue thinking they are the authoritative fracture of the system, leading to consolidation issues down the line. This library does not hold your hand at all solving issues like this, and you're responsible for shaping the anticipated behaviour of your distributed system. Whether or not you're able to push down some of these problems to your data persistence tech of choice is highly dependent on your specific tech stack.

> Note that distributed commit protocols do not solve problems caused by bad architectural decisions; specifically those related to coupling. The greater the conceptual distance between two operations, the worse an idea it is to use a distributed commit (and in extension a service) to couple these to one another. Propagating changes throughout the domain through the use of a [saga](../Saga/Readme.md) would be a better idea.
