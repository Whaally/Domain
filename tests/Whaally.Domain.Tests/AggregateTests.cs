using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions;
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
        var ag = await _aggregateHandlerFactory.Instantiate<TestAggregate>(Guid.NewGuid());

        var result = await ag.Evaluate(
            new TestCommand
            {
                Result = Result.Success()
            });

        result.IsSuccess.Should().BeTrue();
        result.Value?.Messages.Should().BeEmpty();
    }

    [Fact]
    public async Task Aggregate_Command_Evaluation_May_Fail()
    {
        var service = await _aggregateHandlerFactory.Instantiate<TestAggregate>(Guid.NewGuid());

        var operationResult = await service.Evaluate(
            new TestCommand
            {
                Result = Result.Fail("Failure")
            });

        Assert.True(operationResult.IsFailure);
        Assert.Throws<InvalidOperationException>(() => operationResult.Value);
    }

    [Fact]
    public async Task Successful_Command_Evaluation_Provides_Events()
    {
        var aggregateHandler = await _aggregateHandlerFactory.Instantiate<TestAggregate>(Guid.NewGuid());

        var result = await aggregateHandler.Evaluate(
            new TestCommand
            {
                Events = [new TestEvent()],
                Result = Result.Success()
            });

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Messages);
    }

    [Fact]
    public async Task AggregateId_Should_Be_Set_On_Event_Metadata()
    {
        var guid = Guid.NewGuid();
        var aggregateHandler = await _aggregateHandlerFactory.Instantiate<TestAggregate>(guid);

        var result = await aggregateHandler.Evaluate(
            new CommandEnvelope(
                new CommandMetadata
                {
                    AggregateId = guid
                },
                new TestCommand
                {
                    Events = [ new TestEvent() ],
                    Result = Result.Success()
                }));

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Messages);

        result.Value.Metadata.AggregateId.Should().Be(guid);
    }
}
