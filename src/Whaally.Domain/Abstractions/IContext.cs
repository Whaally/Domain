using System.Diagnostics;

namespace Whaally.Domain.Abstractions;

public interface IContext
{
    public ActivityContext? ParentContext { get; }
    
    public IReadOnlyDictionary<string, object> Attributes { get; }
}
