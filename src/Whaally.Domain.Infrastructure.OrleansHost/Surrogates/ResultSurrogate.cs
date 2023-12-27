using FluentResults;

namespace Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

[GenerateSerializer]
public struct ResultSurrogate
{
    [Id(0)] public List<IReason> Reasons;
}