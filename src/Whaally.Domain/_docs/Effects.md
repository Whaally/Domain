# Effects
Effects are used to describe the results from certain operation handlers:

- Saga handlers
- Service handlers
- Command handlers

The effect describes the intended (internal) side effects by these components. An effect can either be a result or a failure. These are mutually exclusive.

One implementation goal of the effect system is to rather unambiguously communicate success or failure states. At the same time it attempts to unambiguously state side effects, without having to manually mutate state.

# Error handling semantics
When a handler returns a failure as its effect result, the handler will be unable to return a validation state on any of its nested dependencies. As to provide a more complete overview of the validation state of the object adjacent handlers will still be evaluated. If any of these fail as well their failure reasons will be appended to the final output.

> Event handlers explicitly do not output effects. The sole role of an event is to mutate the state of an aggregate. To make this explict the aggregate is returned instead.