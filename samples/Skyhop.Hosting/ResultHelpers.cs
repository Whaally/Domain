using System.Text.Json;
using FluentResults;

namespace Skyhop.Hosting;

public static class ResultHelpers
{
    public static Task<IResult> AsResult<TResult>(this IResult<TResult> result)
    {
        if (result.IsFailed)
            return Task.FromResult<IResult>(TypedResults.ValidationProblem(
                result.Reasons.ToDictionary(q => q.Message, q => new string[] { })));

        string? response = null;
        
        response = JsonSerializer.Serialize(result.Reasons);
        
        
        // Using this primitive approach at the moment as the `TypedResults.Json` method
        // does not properly serialize the returned events.
        return Task.FromResult<IResult>(TypedResults.Text(response, "application/json"));
    }
}