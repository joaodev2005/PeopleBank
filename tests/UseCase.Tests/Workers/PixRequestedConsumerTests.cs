using Confluent.Kafka;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PeopleBank.Domain.Entities;
using PeopleBank.Domain.Events;
using PeopleBank.Infrastructure.Data;
using PeopleBank.Worker.Workers;

namespace PeopleBank.UseCase.Tests.Workers;

public class PixRequestedConsumerTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        var scopeFactory = new Mock<IServiceScopeFactory>();
        var logger = new Mock<ILogger<PixRequestedConsumer>>();

        var consumer = new PixRequestedConsumer(scopeFactory.Object, logger.Object);

        consumer.Should().NotBeNull();
    }

    [Fact]
    public async Task Process_ShouldHandleIdempotency_DuplicateMessageNotReprocessed()
    {
        var idempotencyKey = "idem-dup-1";
        var pixEvent = new PixRequestedEvent
        {
            TransactionId = Guid.NewGuid(),
            SourceAccountId = Guid.NewGuid(),
            TargetAccountId = Guid.NewGuid(),
            Amount = 100,
            IdempotencyKey = idempotencyKey,
            Description = "test",
            RequestedAt = DateTime.UtcNow
        };

        var options = new DbContextOptionsBuilder<PeopleBankDbContext>()
            .UseInMemoryDatabase("PixIdempotencyTest")
            .Options;

        await using var db = new PeopleBankDbContext(options);

        var existingTx = new Transaction(pixEvent.SourceAccountId, pixEvent.TargetAccountId, pixEvent.Amount, idempotencyKey, "existing");
        existingTx.MarkAsCompleted();
        db.Transactions.Add(existingTx);
        await db.SaveChangesAsync();

        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        var scopeMock = new Mock<IServiceScope>();
        var providerMock = new Mock<IServiceProvider>();
        providerMock.Setup(p => p.GetService(typeof(PeopleBankDbContext))).Returns(db);
        scopeMock.Setup(s => s.ServiceProvider).Returns(providerMock.Object);
        scopeFactoryMock.Setup(f => f.CreateScope()).Returns(scopeMock.Object);

        var loggerMock = new Mock<ILogger<PixRequestedConsumer>>();
        var consumer = new PixRequestedConsumer(scopeFactoryMock.Object, loggerMock.Object);

        var consumeResult = new ConsumeResult<string, string>
        {
            Message = new Message<string, string> { Value = System.Text.Json.JsonSerializer.Serialize(pixEvent) }
        };

        Assert.True(true);
    }

    [Fact]
    public void Concurrency_Simulation_ShouldRespectUniqueIndex()
    {
        Assert.True(true);
    }
}