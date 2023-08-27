using FluentResults;
using Whaally.Domain.Infrastructure.OrleansMarten.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansMarten.Converters
{
    [RegisterConverter]
    public sealed class ResultConverter :
        IConverter<Result, ResultSurrogate>
    {
        public Result ConvertFromSurrogate(in ResultSurrogate surrogate)
        {
            return new Result().WithReasons(surrogate.Reasons);
        }

        public ResultSurrogate ConvertToSurrogate(in Result value)
            => new ResultSurrogate()
            {
                Reasons = value.Reasons
            };
    }
}
