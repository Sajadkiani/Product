using System.Threading.Tasks;
using EventBus.Abstractions;
using Events;
using Product.Api.Application.Commands.Common;
using Product.Api.Application.Commands.Users;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Product.Api.Application.IntegrationEvents.Consumers.Test
{
    public class TestIntegrationEventConsumer : IConsumer<TestIntegrationEvent>
    {
        private readonly ILogger<TestIntegrationEventConsumer> logger;
        private readonly IEventBus eventBus;

        public TestIntegrationEventConsumer(
            ILogger<TestIntegrationEventConsumer> logger,
            IEventBus eventBus    
        )
        {
            this.eventBus = eventBus;
            this.logger = logger;
        }

        public async Task Consume(ConsumeContext<TestIntegrationEvent> context)
        {
            //TODO: how should we handel the log? 

            var command = new TestCommand { UserName = context.Message.UserName };

            var identifiedLoginCommand = new IdentifiedCommand<TestCommand, bool>(command, context.RequestId.Value);

            //TODO: log maybe was good

            _ = await eventBus.SendMediator(identifiedLoginCommand);
        }
    }
}