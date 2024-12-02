Distributed Commits
===================

The domain library has left some space for the implementation of distributed commit protocols. This is achieved through the combination of various components:

- A custom implementation of the `IEvaluationAgent`
- Custom `IAggregateHandler` and `IAggregateHandlerFactory` implementations

While the evaluation agent is responsible for scheduling and coordinating distributed commits, a custom aggregate handler is required to implement additional behaviour surrounding locks, leader elections, timeouts, aborts and commits. The aggregate handler factory is only necessary to instantiate handlers of the correct (custom implemented) type.

All together this leaves some room to implement a variety of commit protocols, depending on the use cases and requirements.

## Two phase commits
To implement a two phase commit, consider the following approach:

![2PC](https://upload.wikimedia.org/wikipedia/commons/8/86/Two_phase_commit_seq_diagram_success_01.png)

- The coordinator of the operation is the evaluation agent. The physical location of the evaluation agent is dependent on the origin and/or scheduling of the operation.
- Command evaluation is conceptually repurposed as the "prepare" operation. This puts a lock on participants which prevents any other operation from making a change.
  - Participants already return whether or not the requested change is compatible with their local state. If not an abort operation is requested.
  - This lock is held until a commit operation, an abort operation, or a timeout.
- The apply operation is repurposed as the conceptual "commit" operation. This ensures the participants accept the requested changes, and write it to their local state.
  - Execution of the continuation can be an implicit side-effect of the apply operation. Whether to do this depends on the concrete requirements. Each continuation can then be executed as a separate commit operation.

If/when all nodes are responsive, a distributed commit can be achieved in a minimum of 2 round trip times (RTT).

### Weaknesses
- Two phase commits are prone to coordinator failures. Upon failure participants should be able to instantiate a new coordinator (evaluation agent), and achieve consensus about the viability of the requested state change.
- Upon evaluation of services, there is a certain amount of time between any information the service acts upon, and the time the distributed commit of resulting commands happens. Depending on load and interdependence of operations this may increase odds of commit failure. _If timing is that crucial and error-prone you might as well talk about race conditions instead, and go with a completely different design altogether._
- At this moment there is no way to implement or enable commit protocols on a case-by-case basis.
