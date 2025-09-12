using System.ComponentModel.DataAnnotations;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class CommandResultEffect : ICommandEffect, ICommandResult
{
    public ICommandResult Defer(ICommand command)
    {
        throw new NotImplementedException();
    }

    public ICommandResult Stage(IEvent @event)
    {
        throw new NotImplementedException();
    }
}

public class CommandFailureEffect : ICommandEffect, ICommandFailure
{
    public ICommandFailure Fail(ValidationResult validationResult)
    {
        throw new NotImplementedException();
    }
}