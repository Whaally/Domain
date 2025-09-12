using System.ComponentModel.DataAnnotations;

namespace Whaally.Domain.Abstractions;

public interface ISagaEffect : IEffect;

public interface ISagaResult : ISagaEffect, IResult
{
    public ISagaResult Defer(IService service);
    public ISagaResult Stage(string aggregateId, params ICommand[] command);
}

public interface ISagaFailure : ISagaEffect, IFailure
{
    // Sidenote; it's slightly awkward failing sagas, but we need some visibility if systems do not behave the way we expect
    
    public ISagaFailure Fail(ValidationResult validationResult);
    public ISagaFailure Fail(string error) => Fail(new ValidationResult(error));
    public ISagaFailure Fail(string error, string member) => Fail(new ValidationResult(error, [ member ]));
}
