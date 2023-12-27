using FluentResults;
using Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansHost.Converters;

[RegisterConverter]
public sealed class GenericResultConverter<TValue> : IConverter<Result<TValue>, GenericResultSurrogate<TValue>>
{
    public Result<TValue> ConvertFromSurrogate(in GenericResultSurrogate<TValue> surrogate) =>
        new Result<TValue>()
            .WithValue(surrogate.Value)
            .WithReasons(surrogate.Reasons);

    public GenericResultSurrogate<TValue> ConvertToSurrogate(in Result<TValue> value) =>
        new()
        {
            Reasons = value.Reasons,
            Value = value.ValueOrDefault
        };
}