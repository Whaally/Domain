﻿using FluentResults;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Events;
using Whaally.Domain.Abstractions.Command;

namespace Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;

[Immutable]
[GenerateSerializer]
public record SetAircraft(string AircraftId) : ICommand;

public class SetAircraftHandler : ICommandHandler<Flight, SetAircraft>
{
    public IResultBase Evaluate(ICommandHandlerContext<Flight> context, SetAircraft command)
    {
        var result = new Result();

        if (string.IsNullOrWhiteSpace(command.AircraftId))
            result.WithError("Aircraft was not provided");

        if (result.IsSuccess)
        {
            if (!string.IsNullOrWhiteSpace(context.Aggregate.AircraftId))
            {
                context.StageEvent(
                    new AircraftRemoved(
                        context.Aggregate.AircraftId));
            }

            context.StageEvent(
                new AircraftSet(command.AircraftId));
        }

        return result;
    }
}