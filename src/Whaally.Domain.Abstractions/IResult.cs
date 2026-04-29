using System.ComponentModel.DataAnnotations;

namespace Whaally.Domain.Abstractions;

public interface IResult
{
    public bool IsFailure { get; }
    public bool IsSuccess { get; }
    
    IEnumerable<ValidationResult> Errors { get; }
}

public interface IResult<T> : IResult
{   
    T Value { get; }

    T? ValueOrDefault => IsSuccess 
        ? Value 
        : default;
}
