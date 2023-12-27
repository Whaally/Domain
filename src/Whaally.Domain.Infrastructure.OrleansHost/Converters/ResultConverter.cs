using FluentResults;
using Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansHost.Converters;

[RegisterConverter]
public sealed class ResultConverter :
    IConverter<Result, ResultSurrogate>
{
    public Result ConvertFromSurrogate(in ResultSurrogate surrogate) => 
        new Result().WithReasons(surrogate.Reasons);

    public ResultSurrogate ConvertToSurrogate(in Result value) =>
        new()
        {
            Reasons = value.Reasons
        };
}