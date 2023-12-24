using Microsoft.AspNetCore.Mvc;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Whaally.Domain.Abstractions.Aggregate;

namespace Skyhop.Hosting.Controllers;

[ApiController]
[Route("[controller]")]
public class FlightController : ControllerBase
{
    private readonly IAggregateHandlerFactory _aggregateHandlerFactory;
    
    public FlightController(IAggregateHandlerFactory aggregateHandlerFactory)
    {
        _aggregateHandlerFactory = aggregateHandlerFactory;
    }

    [HttpPost("new")]
    public Task<IResult> New()
        => _aggregateHandlerFactory.Instantiate<Flight>(Guid.NewGuid().ToString())
            .EvaluateAndApply(new Create());
    
    [HttpPost("{id}/aircraft/clear")]
    public Task<IResult> RemoveAircraft(string id) =>
        _aggregateHandlerFactory.Instantiate<Flight>(id)
            .EvaluateAndApply(new RemoveAircraft());

    [HttpPost("{id}/aircraft/set")]
    public Task<IResult> SetAircraft(string id, SetAircraftCommand aircraftCommand) =>
        _aggregateHandlerFactory.Instantiate<Flight>(id)
            .EvaluateAndApply(aircraftCommand);

    [HttpPost("{id}/departure/set")]
    public Task<IResult> SetDeparture(string id, SetDeparture departure) =>
        _aggregateHandlerFactory.Instantiate<Flight>(id)
            .EvaluateAndApply(departure);

    [HttpPost("{id}/arrival/set")]
    public Task<IResult> SetArrival(string id, SetArrival arrival) =>
        _aggregateHandlerFactory.Instantiate<Flight>(id)
            .EvaluateAndApply(arrival);
}