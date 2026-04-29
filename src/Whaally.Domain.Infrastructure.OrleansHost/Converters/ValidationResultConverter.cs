using System.ComponentModel.DataAnnotations;
using Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansHost.Converters;

[RegisterConverter]
public sealed class ValidationResultConverter : IConverter<ValidationResult, ValidationResultSurrogate>
{
    public ValidationResult ConvertFromSurrogate(in ValidationResultSurrogate surrogate) =>
        new(surrogate.ErrorMessage, surrogate.MemberNames);

    public ValidationResultSurrogate ConvertToSurrogate(in ValidationResult value)
        => new()
        {
            ErrorMessage = value.ErrorMessage,
            MemberNames = value.MemberNames
        };
}