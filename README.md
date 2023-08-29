# Whaally.Domain

The goal of the `Whaally.Domain` library is to simplify the development of highly-scalable domain models. The way this library attempts to solve this problem is by providing a framework dictating what components exist and how they may interact with one another. The terminology for these components had largely been inspired by the language used by DDD practitioners, as well as the practices used by those working with event-sourcing techniques.

As such this framework defines how to build the following technical components:

- Aggregates
- Services
- Sagas
- Commands
- Events

This library assumes you provide the behaviour for these components. By adhering to a common behavioural pattern this library is able to provide common behaviour integrating this behaviour with the infrastructure required to run it.

The end result is an approach wherein we can focus on the development of business behaviour, without worrying about the integration of infrastructure.

## Example usage
The implementation of a simple aggregate looks like this:



## Interaction pattern
This project is designed to facilitate a coherent interaction pattern within the domain model. 

![Flowchart showing interaction between components](./assets/Domain%20Structure.drawio.svg)

As such; 
- Changes can only be initiated through the invocation of either a command or a service
- Services can only invoke services or commands
- Commands can only express their intended changes to the aggregate through the use of events
- Events are the sole components being able to change the aggregate
- Sagas are similar to services, with the difference that they are triggered by events, rather than external actions

By imposing this explicit structure on the way the domain is expressed we can approach infrastructure as being the glue between these components. This allows us to generalize infrastructure, not having to reinvent the wheel each time we are shipping a new feature.

### Scalability
This model provides beneficial characteristics regarding scalability. Through this model any service invocation can be reduced to be represented by a collection of events. Because of this it becomes possible to compose complex behaviour while still being able to evaluate the validity of the operation and any side-effects. This can all be achieved within two or three round trip times (RTT), depending on the distributed commit protocol used.

### Side effect free programming
Because all intended changes are explicitly modelled it becomes possible to solely consider the happy path within the application. Any evaluation error originating from a command has the capability to undo the pending operation. This allows one to compose complex service compositions without having to roll back all the intended operations upon failure somewhere along the chain. All of this can be handled through the infrastructure.

At the same time this forces one to explicitly model failures as part of the business model, elevating the failure mode to a first class citizen within the domain model, rather than it being an afterthought.

### Composition
With the side-effect free programming model we get an additional benefit; that of composition. Because operations are not applied when a validation error occurs somewhere along the way, we can confidently build further onto existing behaviour. This way we can reuse existing commands and services to achieve high-level objectives with the assurance that no side-effects arise from partially or incorrectly applied operations.

### Testability
The abstractions in this package allow one to test the library in exactly the same manner as they would use it, therefore allowing the tests to function as documentation. Additionally one can test all components in a rather granular manner, asserting whether their outputs are correct. This allows one to compose a test suite which is highly granular while still deriving a great amount of confidence from the correct functioning of the whole.

### Further context
Check out this talk by [@corstian](https://github.com/corstian) from NDC Oslo 2023 providing more context to the conceptual principles underpinning this project.

[![How complex software impacts your cognitive abilities](./assets/NDCOslo2023-video-thumbnail.jpg)](https://www.youtube.com/watch?v=5A22s_QXTRg)


## ⚠️ A work in progress
This is the first public version of a library already running in production. Over time the documentation around this project will be slowly built up, and further documentation and examples will be added.

Hit the ⭐ button and file an issue if you want to move the documentation of a portion of this code base to the front of the queue.
