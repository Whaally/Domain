using System.Diagnostics;

namespace Whaally.Domain.Infrastructure.OrleansHost.Surrogates;

[GenerateSerializer]
public struct ActivityContextSurrogate
{
    [Id(0)] public byte[] TraceId;
    [Id(1)] public byte[] SpanId;
    [Id(2)] public ActivityTraceFlags TraceFlags;
}