using System.Diagnostics;

namespace Whaally.Domain;

public static class ActivityContextExtensions
{
    public static Activity Continue(this ActivityContext activityContext, string operationName) =>
        new Activity(operationName)
            .SetParentId(
                activityContext.TraceId,
                activityContext.SpanId,
                activityContext.TraceFlags)
            .Start();
}