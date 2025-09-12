using System.ComponentModel.DataAnnotations;

namespace Whaally.Domain.Abstractions;

public interface ICommandEffect : IEffect;

public interface ICommandResult : ICommandEffect, IResult
{
    public ICommandResult Defer(ICommand command);
    public ICommandResult Stage(IEvent @event);
}

public interface ICommandFailure : ICommandEffect, IFailure
{
    public ICommandFailure Fail(ValidationResult validationResult);
    public ICommandFailure Fail(string error) => Fail(new ValidationResult(error));
    public ICommandFailure Fail(string error, string member) => Fail(new ValidationResult(error, [ member ]));
}
