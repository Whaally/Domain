# Orleans Integration
This package provides the infrastructure necessary to be able to run the Whaally.Domain library on top of Orleans. This 
facilitates elastic scalability of the domain in production environments.

This package contains:
- [Converters and surrogates](https://learn.microsoft.com/en-us/dotnet/orleans/host/configuration-guide/serialization?pivots=orleans-7-0#surrogates-for-serializing-foreign-types) to be able to serialize domain related objects for messaging between Orleans silos
- An abstract implementation of the `IAggregateHandler` allowing plugging your own persistence method with sensible defaults.
- An implementation of the `AggregateProviderFactory` using Orleans
- A fix for an Orleans bug

## Installation
First, register the Orleans based `AggregateHandlerFactory`: 

```csharp
builder.Services.Remove(
    builder.Services.Single(q => q.ServiceType.IsAssignableTo(typeof(IAggregateHandlerFactory))));

builder.Services.AddSingleton<IAggregateHandlerFactory, AggregateHandlerFactory>();
```

In addition to the standard configuration necessary for setting up an Orleans cluster:

```csharp
// See https://github.com/dotnet/orleans/issues/8157 for more context
siloBuilder.Services.AddSingleton<Factory<IGrainContext, ILogConsistencyProtocolServices>>(serviceProvider =>
{
    var factory = ActivatorUtilities.CreateFactory(typeof(ProtocolServices),
        new[] { typeof(IGrainContext) });
    return arg1 => (ILogConsistencyProtocolServices)factory(serviceProvider, new object[] { arg1 });
});
```

Finally there is a choice to be made regarding the aggregate handler to be used. There is a `DefaultAggregateHandler` implementation
providing default behaviour which can be used in other implementations. While this one works well for testing, it does not
persist anything. To get started quickly:
- Implement the abstract class `AbstractAggregateHandler`
- Use the `Whaally.Domain.Infrastructure.OrleansHost.MartenPersistence` package to use Marten for persistence

## Usage
To use the domain, it is recommended to decorate domain objects used for messaging with Orleans's immutable and generate serializer attributes:

```csharp
[Immutable]
[GenerateSerializer]
```

Generally the objects requiring these are `ICommand` and `IEvent` implementations, and all objects used therein.

In situations where types from external libraries are part of the domain model as well, be mindful to write converters 
and surrogates for these to make Orleans play nice with them; [as document here](https://learn.microsoft.com/en-us/dotnet/orleans/host/configuration-guide/serialization?pivots=orleans-7-0#surrogates-for-serializing-foreign-types).
Examples exist in this repository specifically for the Whaally.Domain library.