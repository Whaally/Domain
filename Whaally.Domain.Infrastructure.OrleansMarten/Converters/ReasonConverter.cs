using FluentResults;
using Whaally.Domain.Infrastructure.OrleansMarten.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansMarten.Converters
{
    [RegisterConverter]
    public sealed class SuccessConverter : IConverter<Success, ReasonSurrogate>
    {
        public Success ConvertFromSurrogate(in ReasonSurrogate surrogate)
            => new Success(surrogate.Message)
                .WithMetadata(surrogate.Metadata);

        public ReasonSurrogate ConvertToSurrogate(in Success value)
            => new ReasonSurrogate
            {
                Message = value.Message,
                Metadata = value.Metadata,
                Reasons = null
            };
    }

    [RegisterConverter]
    public sealed class ErrorConverter : IConverter<Error, ReasonSurrogate>
    {
        Error IConverter<Error, ReasonSurrogate>.ConvertFromSurrogate(in ReasonSurrogate surrogate)
        {
            var error = new Error(surrogate.Message);

            error.CausedBy(surrogate.Reasons);
            error.WithMetadata(surrogate.Metadata);

            return error;
        }

        ReasonSurrogate IConverter<Error, ReasonSurrogate>.ConvertToSurrogate(in Error value)
            => new ReasonSurrogate
            {
                Reasons = value.Reasons,
                Metadata = value.Metadata,
                Message = value.Message
            };
    }
}
