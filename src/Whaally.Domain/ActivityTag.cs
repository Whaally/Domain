namespace Whaally.Domain;

internal static class ActivityTag
{
    /// <summary>
    ///     The type of the message invoking the activity
    /// </summary>
    public const string MessageType = "message-type";

    /// <summary>
    ///     The id of the aggregate involved
    /// </summary>
    public const string AggregateId = "aggregate-id";


    public const string OperationStatus = "operation.status";
    public const string OperationErrors = "operation.errors";
    public const string OperationCommands = "operation.commands";

    public const string DomainMessageTypes = "domain.message.types";
    
    /*
     * Information to collect about domain operations:
     * - Input messages
     * - Output messages
     * - 
     */
}
