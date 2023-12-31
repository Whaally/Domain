﻿using Whaally.Domain.Abstractions.Event;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events
{
    public record DepartureAirfieldSet(
        string AggregateId,
        string AirfieldId) : IEvent;

    internal class DepartureAirfieldSetHandler : IEventHandler<Flight, DepartureAirfieldSet>
    {
        public Flight Apply(IEventHandlerContext<Flight> context, DepartureAirfieldSet @event)
            => context.Aggregate with
            {
                DepartureAirfieldId = @event.AirfieldId
            };
    }
}
