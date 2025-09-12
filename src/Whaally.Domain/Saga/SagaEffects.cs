using System.ComponentModel.DataAnnotations;
using Whaally.Domain.Abstractions;

namespace Whaally.Domain;

public class SagaResult : ISagaResult
{
    public ISagaResult Defer(IService service)
    {
        throw new NotImplementedException();
    }

    public ISagaResult Stage(string aggregateId, params ICommand[] command)
    {
        throw new NotImplementedException();
    }
}

public class SagaFailure : ISagaFailure
{
    public ISagaFailure Fail(ValidationResult validationResult)
    {
        throw new NotImplementedException();
    }
}
