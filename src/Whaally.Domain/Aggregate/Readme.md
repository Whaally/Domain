# Aggregate
The aggregate is a central construct within this library. Conceptually it is similar to the way that the aggregate is understood within the Domain Driven Design (DDD) methodology. Core characteristics of the aggregate is that it holds state and it has an identity. Within this library the aggregate is the sole construct inherently stateful. In addition - similar to DDD - only the aggregate has the authority to make changes to its own state. In the context of this library this translates to a shared responsibility between [commands](../Command/Readme.md) and [events](../Event/Readme.md).

## Usage
An aggregate can be defined as follows:

```csharp
public record Flight : IAggregate
{
    // Information...
}
```

There are no limits to what an aggregate may contain. Aggregates do not need to be serializeable either, as they will not be sent over the wire. Instead an aggregate will be constructed by applying relevant events to this instance. It is for this reason that it may contain everything you want it to contain, from fields to methods. Given the aggregate instance itself is never exposed, one cannot use these methods to directly access the aggregate instance either.

The aggregate's behaviour is contained within the following two constructs:

- [Commands](../Command/Readme.md): Responsible for evaluating whether a given operation is allowed
- [Events](../Event/Readme.md): Responsible for modifying aggregate state with the new information

Together commands and events allow one to change the state of the aggregate.

## Scalability
When running the domain on top of an actor system, a single aggregate instance generally translates to a single actor. Operations against this actor (commands, events) run sequentially. Scalability is achieved through placing the activated aggregate instances across multiple machines in a cluster.