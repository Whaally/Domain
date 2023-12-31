﻿using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Abstractions.Command;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands
{
    public record SetArrival(
        string AggregateId,
        DateTime Time,
        string AirfieldId) : ICommand;

    internal class SetArrivalHandler : ICommandHandler<Flight, SetArrival>
    {
        public IResultBase Evaluate(ICommandHandlerContext<Flight> context, SetArrival command)
        {
            var result = new Result();

            if (string.IsNullOrWhiteSpace(command.AirfieldId))
                result.WithError("Airfield was not provided");

            if (command.Time == DateTime.MinValue
                || command.Time == DateTime.MaxValue)
                result.WithError("Arrival time was not provided");

            if (result.IsSuccess)
            {
                context.StageEvent(new ArrivalTimeSet(
                    command.AggregateId,
                    command.Time));

                context.StageEvent(new ArrivalAirfieldSet(
                    command.AggregateId,
                    command.AirfieldId));
            }

            return result;
        }
    }
}
