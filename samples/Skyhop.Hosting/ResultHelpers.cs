using System.Text.Json;
using Whaally.Domain.Abstractions;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Skyhop.Hosting;

public static class ResultHelpers
{
    public static Task<IResult> AsResult<TResult>(this IResult<TResult> result)
    {
        if (result.IsFailure)
            return Task.FromResult<IResult>(
                TypedResults.ValidationProblem( 
                    result.Errors.ToDictionary(
                        q => q.ErrorMessage!, 
                        q => new string[] { })));

        string? response = null;
        
        response = JsonSerializer.Serialize(result.Errors);
        
        
        // Using this primitive approach at the moment as the `TypedResults.Json` method
        // does not properly serialize the returned events.
        return Task.FromResult<IResult>(TypedResults.Text(response, "application/json"));
    }
}