The Evaluation Agent
====================

The evaluation agent is responsible for coordinating operations across the domain. Operations include running services, commands, events and sagas.

It is important for the evaluation agent to make a clear-cut distinction between the (side effect free) evaluation of commands and services, the (effectful) application of events, and the continuation on events through sagas.

> It is implied that the evaluation of different aspects yield different results, but does not do anything with these results:
> - Service evaluation: a list of commands
> - Command evalution: a list of events
> - Event application: side effects on relevant aggregates
> - Event continuation: saga evaluation (comparable to a service), and consequently a list of commands

Concrete behaviour of the domain is largely up to the domain context (for initiating an operation), and the aggregate handler (for executing an operation in the context of a single aggregate). Upon initiation of an operation through the domain context, the context will invoke methods implemented on the evaluation agent depending on the intent as well as the result of the operation.

The `DomainContext` class largely defers domain operations to the evaluation agent instance registered with the dependency container. Contrary to the `DomainContext`, the evaluation agent does not impose restrictions on whether the evaluated commands belong to the same aggregate types or instances. This constraint on the `DomainContext` is put in place as to promote the use of services and/or commands to implement complex operations.
