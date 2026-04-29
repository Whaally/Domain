using Whaally.Domain.Abstractions;
using Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansHost.Converters;

[RegisterConverter]
public sealed class ResultConverter<T> : IConverter<Result<T>, ResultSurrogate>
{
    public Result<T> ConvertFromSurrogate(in ResultSurrogate surrogate) =>
        new Result<T>(surrogate.Reasons);
    
    public ResultSurrogate ConvertToSurrogate(in Result<T> value) => new ResultSurrogate
    {
        Reasons = value.Errors.ToList()
    };
}
