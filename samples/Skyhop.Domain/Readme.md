# Skyhop Core Domain
This project is a sample implementation of the `Whaally.Domain` library simulating a real-world application. The domain is separated in four core areas;

- Aircraft
- Flight
- Airfield
- Pilot

Three of them (Aircraft, Airfield and Pilot) align with the requirement to maintain distinct logs for each of them. The flight is the connecting context/aggregate, as it is a shared aspect of all of them.

Special in this implementation is that everything revolves around a flight. While it would be possible to give a slightly different meaning to the concept of a flight in all of the three core contexts, this would make up for a confusing domain, and therefore resulting user interface. Instead in the interaction with this domain everything revolves around the flight itself. Any changes made therein will automatically be forwarded to their appropriate contexts, being the aircraft, the airfield and the pilot. In this implementation this happens through extensive use of sagas to keep other aggregate - even those outside of its original bounded context - up to date with changes. _This is one of the reasons many of the operations on the domain are marked as internal._
