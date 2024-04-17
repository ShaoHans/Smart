namespace Smart.EventBus.RabbitMQ;

internal class EventMetaDataProvider
{
    /// <summary>
    /// the key is the event type full name
    /// </summary>
    public Dictionary<string, EventMetaData> MetaDatas { get; set; } = [];
}
