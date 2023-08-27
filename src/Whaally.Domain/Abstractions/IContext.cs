using System.Diagnostics;

namespace Whaally.Domain.Abstractions
{
    public interface IContext
    {
        public ActivityContext Activity { get; init; }
    }
}
