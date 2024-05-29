# Skyhop sample project
This project is a sample of what can be expected in the real world. This project brings together the following components:

- ASP.NET Core API
- Orleans
- Whaally.Domain
- Marten

The combination of these components result in a project where the domain layer runs in a distributed manner on top of a cluster. This is the tight integration between the domain and Orleans. Marten is added to ensure the data captured through the domain is persisted, and the API is added on top such that the domain is accessible from the outside world.

> In this sample the API and the domain are coupled together in Orleans. It is also possible to decouple these to allow independently scaling these as necessary. This all depends on the specific use case, scale and requirements.

Since the API and the domain are elastically scalable, the first component to give in under increased load will most likely be the data store. In a situation where a more traditional database is used (such as PostgreSQL in this example), scaling the application just means provisioning a different instance size.

That the database is a bottleneck doesn't mean that an autoscaling API is completely useless. Quite contrary. For applications with wildly varying load it allows scaling up and down on demand, potentially resulting in a significant cost reduction. At the same time, the nature of Orleans, being an actor system, allows for the API to act as a smart cache. When a flood of operations hits the system, the actor system is the first component taking the load, scaling up as more operations need to be handled, and "drip feeding" operations and queries to the database.


