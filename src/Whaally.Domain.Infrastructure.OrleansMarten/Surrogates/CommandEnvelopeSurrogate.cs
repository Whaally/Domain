using Whaally.Domain.Abstractions;
using Whaally.Domain.Abstractions.Command;

namespace Whaally.Domain.Infrastructure.OrleansMarten.Surrogates
{
    [GenerateSerializer]
    public struct CommandEnvelopeSurrogate<TCommand>
        where TCommand : class, ICommand
    {
        [Id(0)] public TCommand Message;
        [Id(1)] public ICommandMetadata Metadata;
    }
}
