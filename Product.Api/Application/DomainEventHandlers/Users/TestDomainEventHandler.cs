﻿using System.Threading;
using System.Threading.Tasks;
using Events;
using Product.Api.Application.IntegrationEvents;
using Product.Domain.Events.Users;
using MediatR;

namespace Product.Api.Application.DomainEventHandlers.Users;

public class TestDomainEventHandler : INotificationHandler<TestDomainEvent>
{
    private readonly IIntegrationEventService integrationEventService;

    public TestDomainEventHandler(
        IIntegrationEventService integrationEventService
        )
    {
        this.integrationEventService = integrationEventService;
    }
    
    public async Task Handle(TestDomainEvent notification, CancellationToken cancellationToken)
    {
        await integrationEventService.AddAndSaveEventAsync(
            new TestIntegrationEvent { UserName = notification.UserName });
    }
}
