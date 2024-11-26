Aggregate Factory
=================

The aggregate factory is a construct used to be able to create new instances of a given aggregate type.

An implementation of the aggregate factory needs to derive from the `IAggregateFactory` interface, and be registered through the dependency container in order to be usable.

A default implementation exists in the `DefaultAggregateFactory` class. This implementation works for types having a default parameterless constructor.

**Background**  
This construct exists to provide more flexibility when it comes to the instantiation of complex objects having dependencies. While ideally you'd want an aggregate to have no such dependencies, the real world is messy and requires compromise. This construct allows one to use interfaces to describe aggregate types, deferring instantiation to such factory implementation.