using System.Reflection;
using Autofac;
using EventBus.Abstractions;
using Identity.Infrastructure.Dapper;
using IntegrationEventLogEF.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Product.Api.Application.DomainEventHandlers.Users;
using Product.Domain.IServices;
using Product.Domain.Validations.Users;
using Product.Infrastructure.Clients.Grpc;
using Product.Infrastructure.ORM.BcValidations;
using Product.Infrastructure.ORM.Dapper;
using Product.Infrastructure.Utils;

namespace Product.Api.AutofacModules;

public class ApplicationModule
    : Autofac.Module
{
    private readonly string dapperConnectionString;

    public ApplicationModule(string dapperConnectionString)
    {
        this.dapperConnectionString = dapperConnectionString;
    }

    protected override void Load(ContainerBuilder builder)
    {

        //TODO: add circuit breaker and retry pattern if it will need AddHttpClient
        //https://learn.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-circuit-breaker-pattern
        
        builder.RegisterType<IdentityGrpcClientImpl>().As<IIdentityClient>().InstancePerLifetimeScope();
        builder.RegisterType<EventInitializer>().As<IEventInitializer>().InstancePerLifetimeScope();
        builder.RegisterType<AppRandoms>().As<IAppRandoms>().InstancePerLifetimeScope();
        builder.RegisterType<Infrastructure.Brokers.EventBus>().As<IEventBus>().InstancePerLifetimeScope();
        builder.RegisterType<DapperQueryExecutor>().As<IQueryExecutor>().InstancePerLifetimeScope();
        builder.RegisterType<CurrentUser>().As<ICurrentUser>().InstancePerLifetimeScope();
        builder.RegisterType<DapperContext>().WithParameter(new TypedParameter(typeof(string), dapperConnectionString));
        
        builder.RegisterAssemblyTypes(
                Assembly.GetAssembly(typeof(IUserStore<>))!, Assembly.GetAssembly(typeof(UserStore))!)
            .Where(t => t.Name.EndsWith("Store"))
            .AsImplementedInterfaces().InstancePerLifetimeScope();
        
        //Inject bounded context validations
        builder.RegisterAssemblyTypes(
                Assembly.GetAssembly(typeof(IProductBcScopeValidation))!, Assembly.GetAssembly(typeof(UserBcScopeValidation))!)
            .Where(t => t.Name.EndsWith("Validation"))
            .AsImplementedInterfaces().InstancePerLifetimeScope();
        
        // Register the DomainEventHandler classes (they implement INotificationHandler<>) in assembly holding the Domain Events
        builder.RegisterAssemblyTypes(typeof(TestDomainEventHandler).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(INotificationHandler<>)).InstancePerLifetimeScope();
    }
}
