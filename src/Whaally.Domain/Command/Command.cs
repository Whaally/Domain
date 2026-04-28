using System.ComponentModel.DataAnnotations;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class Command
{
    public static ICommandResult Result => new CommandResult();
    
    public static ICommandResult Invoke(ICommand command)
    {
        var result = new CommandResult();
        result.Invoke(command);
        return result;
    }

    public static ICommandResult Stage(IEvent @event)
    {
        var result = new CommandResult();
        result.Stage(@event);
        return result;
    }

    public static ICommandResult WithError(string error, params string[] fields)
    {
        var result = new CommandResult();
        result.WithError(error, fields);
        return result;
    }
}

public class CommandResult : ICommandResult
{
    private List<IMessageEnvelope> _operations = new();
    public IEnumerable<IMessageEnvelope> Operations => this._operations;

    public ICommandResult Invoke(ICommand command)
    {
        _operations.Add(
            new CommandEnvelope(
                new CommandMetadata(), 
                command));
        
        return this;
    }

    public ICommandResult Stage(IEvent @event)
    {
        _operations.Add(
            new EventEnvelope(
                new EventMetadata(), 
                @event));
        
        return this;
    }

    public ICommandResult WithError(string error, params string[] fields)
    {
        Errors.Add(new ValidationResult(error, fields));
        return this;
    }

    public bool IsFailure => Errors.Any(q => q != ValidationResult.Success);
    public bool IsSuccess => !IsFailure;

    public List<ValidationResult> Errors { get; } = [];
    IEnumerable<ValidationResult> IResult.Errors => this.Errors;
}
