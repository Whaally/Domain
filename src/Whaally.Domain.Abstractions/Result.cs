using System.ComponentModel.DataAnnotations;

// todo: ideally you'd want to move this object to the domain project instead

namespace Whaally.Domain.Abstractions;

public class Result(IEnumerable<ValidationResult> errors) : IResult
{
    public IEnumerable<ValidationResult> Errors { get; } = errors;

    public static Result Success() => new([]);
    public static Result Fail(string reason) => new([ new ValidationResult(reason) ]);
    
    public bool IsFailure => Errors.Any(q => q != ValidationResult.Success);
    public bool IsSuccess => !IsFailure;
}

public class Result<T> : Result, IResult<T>
{
    public static Result<T> Success(T value) => new(value);
    public new static Result<T> Fail(string reason) => new([ new ValidationResult(reason) ]);

    public Result(IEnumerable<ValidationResult> errors) : base(errors)
    {
        if (IsSuccess) throw new InvalidOperationException("Result indicates no error yet lacks value");
    }

    public Result(T value) : base([])
    {
        _value = value;
    }

    public Result(T value, IEnumerable<ValidationResult> errors) : base(errors)
    {
        if (IsSuccess) _value = value;
    }

    private readonly T? _value;
    public T Value => _value ?? throw new InvalidOperationException();
}