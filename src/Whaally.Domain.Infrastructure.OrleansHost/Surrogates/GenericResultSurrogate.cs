using FluentResults;

namespace Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

[GenerateSerializer]
public struct GenericResultSurrogate<T>
{
    [Id(0)] public List<IReason> Reasons;
    [Id(1)] public T Value;
}