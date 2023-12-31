﻿using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Abstractions.Command;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands
{
    public record SetDeparture(
        string AggregateId,
        DateTime Time,
        string AirfieldId) : ICommand;

    internal class SetDepartureHandler : ICommandHandler<Flight, SetDeparture>
    {
        public IResultBase Evaluate(ICommandHandlerContext<Flight> context, SetDeparture command)
        {
            var result = new Result();

            if (string.IsNullOrWhiteSpace(command.AirfieldId))
                result.WithError("Airfield was not provided");

            if (command.Time == DateTime.MinValue
                || command.Time == DateTime.MaxValue)
                result.WithError("Departure time was not provided");

            if (result.IsSuccess)
            {
                context.StageEvent(new DepartureTimeSet(
                    command.AggregateId,
                    command.Time));

                context.StageEvent(new DepartureAirfieldSet(
                    command.AggregateId,
                    command.AirfieldId));
            }

            return result;
        }
    }
}
