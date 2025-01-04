using Whaally.Domain.Abstractions;

namespace Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

[GenerateSerializer]
public struct CommandEnvelopeSurrogate
{
    [Id(0)] public CommandMetadata Metadata;
    [Id(1)] public IEnumerable<ICommand> Messages;
}
