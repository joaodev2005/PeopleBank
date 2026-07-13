namespace PeopleBank.Domain.Interfaces.Services;

public interface IKafkaProducer
{
    Task PublishAsync<T>(string topic, string key, T message) where T : class;
}
