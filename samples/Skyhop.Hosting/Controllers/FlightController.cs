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
    
    [HttpPost("{id}/aircraft/clear")]
    public Task<IResult> RemoveAircraft(string id) =>
        _aggregateHandlerFactory.Instantiate<Flight>(id)
            .EvaluateAndApply(new RemoveAircraft());

    [HttpPost("{id}/aircraft/set")]
    public Task<IResult> SetAircraft(string id, SetAircraft aircraft) =>
        _aggregateHandlerFactory.Instantiate<Flight>(id)
            .EvaluateAndApply(aircraft);

    [HttpPost("{id}/departure/set")]
    public Task<IResult> SetDeparture(string id, SetDeparture departure) =>
        _aggregateHandlerFactory.Instantiate<Flight>(id)
            .EvaluateAndApply(departure);

    [HttpPost("{id}/arrival/set")]
    public Task<IResult> SetArrival(string id, SetArrival arrival) =>
        _aggregateHandlerFactory.Instantiate<Flight>(id)
            .EvaluateAndApply(arrival);
}