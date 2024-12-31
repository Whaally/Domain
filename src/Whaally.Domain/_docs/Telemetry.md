Telemetry
=========

This library uses an `ActivitySource` to provide telemetry about its behaviour. When using the default configuration, you can add "Whaally.Domain" source to collection telemetry from the domain library.

## Tracing in practice
To get a deep insight into the runtime behaviour of the domain we're tracing all operation form the context provided to handlers.

A major benefit of doing so is that these context instances are easily created locally on the node running the handler.