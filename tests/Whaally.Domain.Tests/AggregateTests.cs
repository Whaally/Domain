using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions.Aggregate;
using Whaally.Domain.Command;
using Whaally.Domain.Tests.Domain;

namespace Whaally.Domain.Tests;

public class AggregateTests
{
    readonly IServiceProvider _services = DependencyContainer.Create();
    private readonly IAggregateHandlerFactory _aggregateHandlerFactory;

    public AggregateTests()
    {
        _aggregateHandlerFactory = _services.GetRequiredService<IAggregateHandlerFactory>();
    }

    [Fact]
    public async Task Aggregate_Accepts_Command()
    {
        var service = _aggregateHandlerFactory.Instantiate<TestAggregate>(Guid.NewGuid().ToString());

        var result = await service.Evaluate(
            new TestCommand
            {
                Result = Result.Ok()
            });

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Aggregate_Command_Evaluation_May_Fail()
    {
        var service = _aggregateHandlerFactory.Instantiate<TestAggregate>(Guid.NewGuid().ToString());

        var operationResult = await service.Evaluate(
            new TestCommand
            {
                Result = Result.Fail("Failure")
            });

        Assert.True(operationResult.IsFailed);
        Assert.Throws<InvalidOperationException>(() => operationResult.Value);
    }

    [Fact]
    public async Task Successful_Command_Evaluation_Provides_Events()
    {
        var aggregateHandler = _aggregateHandlerFactory.Instantiate<TestAggregate>(Guid.NewGuid().ToString());

        var result = await aggregateHandler.Evaluate(
            new TestCommand
            {
                Events = new[] { new TestEvent() },
                Result = Result.Ok()
            });

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
    }

    [Fact]
    public async Task AggregateId_Should_Be_Set_On_Event_Metadata()
    {
        var guid = Guid.NewGuid().ToString();
        var aggregateHandler = _aggregateHandlerFactory.Instantiate<TestAggregate>(guid);

        var result = await aggregateHandler.Evaluate(
            new TestCommand
            {
                Events = new[] { new TestEvent() },
                Result = Result.Ok()
            });

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal(guid, result.Value.First().Metadata.AggregateId);
    }
}
