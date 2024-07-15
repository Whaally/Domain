using System.Text.Json;
using Microsoft.Azure.Functions.Worker;

namespace Skyhop.Domain.AzureFunctions;

public class AggregateHandler 
{
    [Function("AggregateHandler")]
    public static Task DispatchAsync([EntityTrigger] TaskEntityDispatcher dispatcher)
    {
        return dispatcher.DispatchAsync(operation =>
        {
            var identity = JsonSerializer.Deserialize<AggregateIdentity>(operation.Context.Id.Key);

            if (identity == null
                || identity.Id == Guid.Empty
                || identity.Type != "Aircraft") throw new Exception();
            
            return default;
        });
    }
}
