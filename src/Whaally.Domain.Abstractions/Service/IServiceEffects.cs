using System.ComponentModel.DataAnnotations;

namespace Whaally.Domain.Abstractions;

public interface IServiceEffect : IEffect;

public interface IServiceResult : IServiceEffect, IResult
{
    public IServiceResult Defer(IService service);
    public IServiceResult Stage(string aggregateId, params ICommand[] command);
}

public interface IServiceFailure : IServiceEffect, IFailure
{
    public IServiceFailure Fail(ValidationResult validationResult);
    public IServiceFailure Fail(string error) => Fail(new ValidationResult(error));
    public IServiceFailure Fail(string error, string member) => Fail(new ValidationResult(error, [ member ]));
}
