﻿using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events
{
    public record AircraftRemoved(
        string AggregateId,
        string AircraftId) : IEvent;

    public class AircraftRemovedHandler : IEventHandler<Flight, AircraftRemoved>
    {
        public Flight Apply(IEventHandlerContext<Flight> context, AircraftRemoved @event)
            => context.Aggregate with
            {
                AircraftId = null,
                AircraftRegistration = null
            };
    }
}
