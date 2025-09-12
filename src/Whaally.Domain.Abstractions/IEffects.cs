namespace Whaally.Domain.Abstractions;

public interface IEffect;
public interface IResult : IEffect;
public interface IFailure : IEffect;