using System.Diagnostics;

namespace Whaally.Domain;

public static class ActivityContextExtensions
{
    public static Activity? Continue(this ActivityContext? activityContext, string operationName)
    {
        if (activityContext != null)
            return new Activity(operationName)
                .SetParentId(
                    activityContext.Value.TraceId,
                    activityContext.Value.SpanId,
                    activityContext.Value.TraceFlags)
                .Start();

        return null;
    }
}