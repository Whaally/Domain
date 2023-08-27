using FluentResults;
using Whaally.Domain.Infrastructure.OrleansMarten.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansMarten.Converters
{

    [RegisterConverter]
    public sealed class GenericResultConverter<TValue> : IConverter<Result<TValue>, GenericResultSurrogate<TValue>>
    {
        public Result<TValue> ConvertFromSurrogate(in GenericResultSurrogate<TValue> surrogate)
            => new Result<TValue>()
                .WithValue(surrogate.Value)
                .WithReasons(surrogate.Reasons);

        public GenericResultSurrogate<TValue> ConvertToSurrogate(in Result<TValue> value)
            => new GenericResultSurrogate<TValue>()
            {
                Reasons = value.Reasons,
                Value = value.ValueOrDefault
            };
    }
}
