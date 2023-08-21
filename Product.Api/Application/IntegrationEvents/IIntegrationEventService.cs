using System;
using System.Threading.Tasks;

namespace Product.Api.Application.IntegrationEvents;

public interface IIntegrationEventService
{
    Task PublishEventsThroughEventBusAsync(Guid transactionId);
    Task AddAndSaveEventAsync<TEvent>(TEvent evt);
}