﻿using System;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Extensions;
using Product.Api.Application.IntegrationEvents;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Product.Infrastructure.Data.EF;
using Serilog.Context;

namespace Product.Api.Application.Behaviors;

public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> logger;

    private readonly AppDbContext dbContext;

    private readonly IIntegrationEventService integrationEventService;
    // private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

    public TransactionBehaviour(
        AppDbContext dbContext,
        IIntegrationEventService integrationEventService,
        ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
    {
        this.dbContext = dbContext ?? throw new ArgumentException(nameof(dbContext));
        this.integrationEventService = integrationEventService ?? throw new ArgumentException(nameof(integrationEventService));
        this.logger = logger ?? throw new ArgumentException(nameof(ILogger));
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = default(TResponse);
        var typeName = request.GetGenericTypeName();

        try
        {
            if (dbContext.HasActiveTransaction)
            {
                return await next();
            }

            var strategy = dbContext.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                Guid transactionId;

                await using var transaction = await dbContext.BeginTransactionAsync();
                using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                {
                    logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})",
                        transaction.TransactionId, typeName, request);

                    response = await next();

                    logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}",
                        transaction.TransactionId, typeName);

                    await dbContext.CommitTransactionAsync(transaction);

                    transactionId = transaction.TransactionId;
                }

                await integrationEventService.PublishEventsThroughEventBusAsync(transactionId);
            });

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);

            throw;
        }
    }
}