using System.ComponentModel.DataAnnotations;

namespace Whaally.Domain;

public class DomainValidationResult : ValidationResult
{
    protected DomainValidationResult(ValidationResult validationResult) : base(validationResult)
    {
    }

    public DomainValidationResult(string? errorMessage) : base(errorMessage)
    {
    }

    public DomainValidationResult(string? errorMessage, IEnumerable<string>? memberNames) : base(errorMessage, memberNames)
    {
    }
}