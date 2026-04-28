using System.ComponentModel.DataAnnotations;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class Saga
{
    public static ISagaResult Result => new SagaResult(); 
    
    public static ISagaResult Invoke(IService service)
    {
        var result = new SagaResult();
        result.Invoke(service);
        return result;
    }

    public static ISagaResult Stage(Guid aggregateId, params ICommand[] commands)
    {
        var result = new SagaResult();
        result.Stage(aggregateId, commands);
        return result;
    }

    public static ISagaResult WithError(string error, params string[] fields)
    {
        var result = new SagaResult();
        result.Errors.Add(new ValidationResult(error, fields));
        return result;
    }
}

public class SagaResult : ISagaResult
{
    private List<IMessageEnvelope> _operations = new();
    public IEnumerable<IMessageEnvelope> Operations => _operations.AsReadOnly();

    public ISagaResult Invoke(IService service)
    {
        _operations.Add(
            new ServiceEnvelope(
                new ServiceMetadata(),
                service));
        
        return this;
    }

    public ISagaResult Stage(Guid aggregateId, params ICommand[] command)
    {
        _operations.Add(
            new CommandEnvelope(
                new CommandMetadata()
                {
                    AggregateId = aggregateId
                }
                , command));
        
        return this;
    }
    
    public bool IsFailure => Errors.Any(q => q != ValidationResult.Success);
    public bool IsSuccess => !IsFailure;

    public List<ValidationResult> Errors { get; } = [];
    IEnumerable<ValidationResult> IResult.Errors => this.Errors;
}
