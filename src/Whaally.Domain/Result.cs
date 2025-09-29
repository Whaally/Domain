using FluentResults;

namespace Whaally.Domain;

public class Result<TSelf> : ResultBase<TSelf>
    where TSelf : Result<TSelf>, new()
{
    /// <summary>
    ///     Default constructor
    /// </summary>
    public Result()
    {
    } 
}