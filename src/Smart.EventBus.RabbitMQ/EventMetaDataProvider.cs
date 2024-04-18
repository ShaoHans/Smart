namespace Smart.EventBus.RabbitMQ;

internal class EventMetaDataProvider
{
    /// <summary>
    /// the key is the routingkey
    /// </summary>
    public static Dictionary<string, EventMetaData> MetaDatas { get; set; } = [];

    public static KeyValuePair<string, EventMetaData> GetMetaData(Type eventType)
    {
        return MetaDatas.FirstOrDefault(m => m.Value.EventType == eventType);
    }

    public static IEnumerable<(string name, string type)> GroupExchange()
    {
        return MetaDatas
            .GroupBy(m => new { m.Value.ExchangeName, m.Value.ExchangeType })
            .Select(g => (g.Key.ExchangeName!, g.Key.ExchangeType!));
    }

    public static IEnumerable<string> GroupQueue()
    {
        return MetaDatas
           .GroupBy(m => new { m.Value.QueueName })
           .Select(g => g.Key.QueueName!);
    }
}
