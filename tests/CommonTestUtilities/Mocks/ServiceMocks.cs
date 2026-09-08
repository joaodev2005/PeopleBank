using Moq;
using PeopleBank.Domain.Interfaces.Services;

namespace CommonTestUtilities.Mocks;

public static class ServiceMocks
{
    public static Mock<IKafkaProducer> CreateKafkaProducer()
    {
        var mock = new Mock<IKafkaProducer>();
        mock.Setup(k => k.PublishAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<object>()))
            .Returns(Task.CompletedTask);
        return mock;
    }

    public static Mock<IRedisService> CreateRedisService()
    {
        var mock = new Mock<IRedisService>();

        mock.Setup(r => r.AcquireLockAsync(It.IsAny<string>(), It.IsAny<decimal>()))
            .ReturnsAsync(true);

        mock.Setup(r => r.ReleaseLockAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        mock.Setup(r => r.SetIdempotentResponseAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        return mock;
    }
}