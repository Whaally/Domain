using System.Diagnostics;
using System.Text;
using Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

namespace Whaally.Domain.Infrastructure.OrleansHost.Converters;

[RegisterConverter]
public sealed class ActivityContextConverter : IConverter<ActivityContext, ActivityContextSurrogate>
{
    public ActivityContext ConvertFromSurrogate(in ActivityContextSurrogate surrogate)
        => new ActivityContext(
            ActivityTraceId.CreateFromUtf8String(surrogate.TraceId),
            ActivitySpanId.CreateFromUtf8String(surrogate.SpanId),
            surrogate.TraceFlags);

    public ActivityContextSurrogate ConvertToSurrogate(in ActivityContext value)
        => new()
        {
            TraceId = Encoding.UTF8.GetBytes(value.TraceId.ToHexString()),
            SpanId = Encoding.UTF8.GetBytes(value.SpanId.ToHexString()),
            TraceFlags = value.TraceFlags
        };
}