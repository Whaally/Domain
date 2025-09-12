using System.ComponentModel.DataAnnotations;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class ServiceResultEffect : IServiceResult
{
    public IServiceResult Defer(IService service)
    {
        throw new NotImplementedException();
    }

    public IServiceResult Stage(string aggregateId, params ICommand[] command)
    {
        throw new NotImplementedException();
    }
}

public class ServiceFailureEffect : IServiceFailure
{
    public IServiceFailure Fail(ValidationResult validationResult)
    {
        throw new NotImplementedException();
    }
}
