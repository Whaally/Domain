using System.ComponentModel.DataAnnotations;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class Service
{
    public IServiceResult Result => new ServiceResult();

    public IServiceResult Invoke(IService service)
    {
        var result = new ServiceResult();
        result.Invoke(service);
        return result;
    }
    
    public IServiceResult Stage(Guid aggregateId, params ICommand[] commands)
    {
        var result = new ServiceResult();
        result.Stage(aggregateId, commands);
        return result;
    }
    
    public IServiceResult WithError(string error, params string[] fields)
    {
        var result = new ServiceResult();
        result.Errors.Add(new ValidationResult(error, fields));
        return result;
    }
}

public class ServiceResult : IServiceResult
{
    private List<IMessageEnvelope> _operations = new();
    public IEnumerable<IMessageEnvelope> Operations => _operations.AsReadOnly();

    public IServiceResult Invoke(IService service)
    {
        _operations.Add(
            new ServiceEnvelope(
                new ServiceMetadata(), 
                service));
        
        return this;
    }

    public IServiceResult Stage(Guid aggregateId, params ICommand[] command)
    {
        _operations.Add(
            new CommandEnvelope(
                new CommandMetadata()
                {
                    AggregateId = aggregateId
                    // todo: add aggregate type for verbosity
                }, 
                command));
        
        return this;
    }
    
    public bool IsFailure => Errors.Any(q => q != ValidationResult.Success);
    public bool IsSuccess => !IsFailure;

    public List<ValidationResult> Errors { get; } = [];
    IEnumerable<ValidationResult> IResult.Errors => this.Errors;
}
