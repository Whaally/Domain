using Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansHost.Converters;

[RegisterConverter]
public sealed class GenericResultConverter<TValue> : IConverter<FluentResults.Result<TValue>, GenericResultSurrogate<TValue>>
{
    public FluentResults.Result<TValue> ConvertFromSurrogate(in GenericResultSurrogate<TValue> surrogate) =>
        new FluentResults.Result<TValue>()
            .WithValue(surrogate.Value)
            .WithReasons(surrogate.Reasons);
    
    public GenericResultSurrogate<TValue> ConvertToSurrogate(in FluentResults.Result<TValue> value) =>
        new()
        {
            Reasons = value.Reasons,
            Value = value.ValueOrDefault
        };
}