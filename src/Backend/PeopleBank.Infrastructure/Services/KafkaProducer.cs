using System.Text.Json;
using Confluent.Kafka;
using PeopleBank.Domain.Interfaces.Services;

namespace PeopleBank.Infrastructure.Services;

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly string _bootstrapServers;

    public KafkaProducer(string bootstrapServers)
    {
        _bootstrapServers = bootstrapServers;
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 1000
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<T>(string topic, string key, T message) where T : class
    {
        var json = JsonSerializer.Serialize(message);

        Console.WriteLine($"Publicando no tópico {topic}: {json}");

        var kafkaMessage = new Message<string, string>
        {
            Key = key,
            Value = json,
            Headers = new Headers
            {
                { "event-type", System.Text.Encoding.UTF8.GetBytes(typeof(T).Name) },
                { "timestamp", System.Text.Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("O")) }
            }
        };

        var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage);
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}