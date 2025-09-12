using System.Text.Json;
using Whaally.Domain;
using Whaally.Domain.Abstractions;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Skyhop.Hosting;

public static class DomainHelpers
{
    public static async Task<IResult> EvaluateAndApply<TAggregate>(
        this IAggregateHandler<TAggregate>? aggregate,
        params ICommand[] commands)
        where TAggregate : class, IAggregate
    {
        if (aggregate == null) return TypedResults.NotFound();

        var result = await aggregate.Evaluate(commands);

        if (result.IsFailed)
            return TypedResults.ValidationProblem(
                result.Reasons.ToDictionary(q => q.Message, q => new string[] { }));

        if (!result.Value.Messages.Any())
            return TypedResults.Ok();
        
        await aggregate.Apply(result.Value);

        var response = JsonSerializer.Serialize<object[]>(
            result.Value.Messages.ToArray());

        // Using this primitive approach at the moment as the `TypedResults.Json` method
        // does not properly serialize the returned events.
        return TypedResults.Text(response, "application/json");
    }

    public static async Task<IResult> EvaluateAndApply<TAggregate>(
        this Task<IAggregateHandler<TAggregate>?> aggregateHandler, 
        params ICommand[] commands)
        where TAggregate : class, IAggregate
        => await (await aggregateHandler).EvaluateAndApply(commands);
}
