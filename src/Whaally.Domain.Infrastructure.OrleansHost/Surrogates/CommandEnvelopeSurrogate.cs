using Whaally.Domain.Abstractions;
using Whaally.Domain.Abstractions.Command;

namespace Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

[GenerateSerializer]
public struct CommandEnvelopeSurrogate
{
    [Id(0)] public ICommand Message;
    [Id(1)] public ICommandMetadata Metadata;
}

[GenerateSerializer]
public struct CommandEnvelopeSurrogate<TCommand>
    where TCommand : class, ICommand
{
    [Id(0)] public TCommand Message;
    [Id(1)] public ICommandMetadata Metadata;
}
