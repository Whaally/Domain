Aggregate Handlers
==================

The aggregate handler is one of the most central and often used component within the domain library. All operations involving an aggregate instance do flow through an aggregate handler. The handler itself encapsulates the aggregate itself, as it is the aggregate which encapsulates the state. In relation to the aggregate, the aggregate handler does not hold state, but instead contains behaviour about how to interact with the state. It is through this distinction that we are able to generalize behaviour involving aggregates.

For this reason the aggregate handler holds behaviour to:
- Evaluate a command
- Apply an event
- Create a snapshot

## The Aggregate Handler Factory
To allow a certain amount of flexibility in the way the handler interacts with the state, handler instances are retrieved through an aggregate handler factory (see the `IAggregateHandlerFactory` type). To use your own aggregate handler, you should also implement a factory for this type and register the factory with the DI container.

## Assumptions on network interaction
Asynchronous operations may happen at various points in the lifecycle of the aggregate handler. First off it is possible to instantiate a handler asynchronously through the aggregate handler factory, thus allowing one to make network roundtrips to retrieve information (if necessary). Additionally, the operations as implemented on the handler itself are all asynchronous, allowing one to interact with network resources upon evaluation, application or when taking snapshots of the object.

