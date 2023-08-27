using Microsoft.Extensions.DependencyInjection;
using Whaally.Domain.Abstractions.Command;
using Whaally.Domain.Abstractions.Service;
using Whaally.Domain.Tests.Domain;

namespace Whaally.Domain.Tests
{
    public class ServiceProviderExtensionTests
    {
        [Fact]
        public void TestGetCommandAggregateType()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<ICommandHandler, TestCommandHandler>()
                .AddTransient<ICommandHandler<TestAggregate, TestCommand>, TestCommandHandler>()
                .BuildServiceProvider();

            var aggregateType = serviceProvider.GetRelatedAggregateTypeForCommand<TestCommand>();

            Assert.Equal(typeof(TestAggregate), aggregateType);
        }

        [Fact]
        public void Test_GetCommandAggregateType_Fails_Without_Non_Generic_Interface()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<ICommandHandler<TestAggregate, TestCommand>, TestCommandHandler>()
                .BuildServiceProvider();

            var aggregateType = serviceProvider.GetRelatedAggregateTypeForCommand<TestCommand>();

            Assert.Null(aggregateType);
        }

        [Fact]
        public void Test_GetCommandAggregateType_Succeeds_Without_Generic_DI_Registration()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<ICommandHandler, TestCommandHandler>()
                .BuildServiceProvider();

            var aggregateType = serviceProvider.GetRelatedAggregateTypeForCommand<TestCommand>();

            Assert.Equal(typeof(TestAggregate), aggregateType);
        }

        [Fact]
        public void Test_GetCommandHandlerForCommand()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient<ICommandHandler, TestCommandHandler>()
                .AddTransient<ICommandHandler<TestAggregate, TestCommand>, TestCommandHandler>()
                .BuildServiceProvider();

            var handler = serviceProvider.GetCommandHandlerForCommand<TestCommand>();

            Assert.IsType<TestCommandHandler>(handler);
        }

        //[Fact]
        //public void Test_GetAggregateHandlerForAggregateType()
        //{
        //    var serviceProvider = new ServiceCollection()
        //        .AddSingleton<IAggregateBehaviourProvider, DefaultAggregateBehaviourProvider>()
        //        .AddTransient<IAggregateHandler, TestAggregateHandler<TestAggregate>>()
        //        .AddTransient<IAggregateHandler<TestAggregate>, TestAggregateHandler<TestAggregate>>()
        //        .BuildServiceProvider();

        //    var handler = serviceProvider.GetAggregateHandlerForAggregateType<TestAggregate>();

        //    Assert.IsType<TestAggregateHandler<TestAggregate>>(handler);
        //}

        [Fact]
        public void TestGetServiceHandlerForService()
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IServiceHandler, TestServiceHandler>()
                .AddSingleton<IServiceHandler<TestService>, TestServiceHandler>()
                .BuildServiceProvider();

            var serviceHandler = serviceProvider.GetServiceHandlerForService(typeof(TestService));

            Assert.IsType<TestServiceHandler>(serviceHandler);
        }
    }
}
