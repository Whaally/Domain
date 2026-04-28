using Microsoft.AspNetCore.Mvc;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate;
using Skyhop.Domain.FlightContext.Aggregates.FlightAggregate.Commands;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Skyhop.Hosting.Controllers;

[ApiController]
[Route("[controller]")]
public class FlightController : ControllerBase
{
    private readonly IAggregateHandlerFactory _aggregateHandlerFactory;
    private readonly DomainContext _domainContext;
    
    public FlightController(
        IAggregateHandlerFactory aggregateHandlerFactory,
        DomainContext domainContext)
    {
        _aggregateHandlerFactory = aggregateHandlerFactory;
        _domainContext = domainContext;
    }

    // [HttpPost("new")]
    // public Task<IResult> New()
    //     => _aggregateHandlerFactory.Instantiate<Flight>(Guid.NewGuid().ToString())
    //         .EvaluateAndApply(new Create());

    [HttpPost("new")]
    public async Task<IResult> New()
    {
        // Trigger or invoke?
        var result = await _domainContext.Invoke(Guid.NewGuid(), new Create());

        return await result.AsResult();
    }
    
    [HttpPost("{id}/aircraft/clear")]
    public Task<IResult> RemoveAircraft(Guid id) =>
        _aggregateHandlerFactory.Instantiate<Flight>(id)
            .EvaluateAndApply(new RemoveAircraft());

    [HttpPost("{id}/aircraft/set")]
    public Task<IResult> SetAircraft(Guid id, SetAircraft aircraftCommand) =>
        _aggregateHandlerFactory.Instantiate<Flight>(id)
            .EvaluateAndApply(aircraftCommand);

    [HttpPost("{id}/departure/set")]
    public Task<IResult> SetDeparture(Guid id, SetDeparture departure) =>
        _aggregateHandlerFactory.Instantiate<Flight>(id)
            .EvaluateAndApply(departure);

    [HttpPost("{id}/arrival/set")]
    public Task<IResult> SetArrival(Guid id, SetArrival arrival) =>
        _aggregateHandlerFactory.Instantiate<Flight>(id)
            .EvaluateAndApply(arrival);
}