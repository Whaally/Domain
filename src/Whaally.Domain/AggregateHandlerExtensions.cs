using FluentResults;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Event;

namespace Whaally.Domain;

public static class AggregateHandlerExtensions
{
    public static async Task<IResult<IEventEnvelope[]>> EvaluateAndApply(this IAggregateHandler aggregateHandler, params ICommand[] commands)
    {
        var evaluationResult = await aggregateHandler.Evaluate(commands);

        if (!evaluationResult.IsSuccess) 
            return Result.Fail<IEventEnvelope[]>(evaluationResult.Errors);
        
        await aggregateHandler.Apply(evaluationResult.Value);
        return Result.Ok(evaluationResult.Value);
    }
}