using System;
using System.Reflection;
using System.Text;
using Identity.Api.Extensions;
using Identity.Infrastructure.Extensions.Options;
using IdentityGrpcClient;
using IntegrationEventLogEF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Product.Api.Extensions;
using Product.Api.Security;
using Product.Infrastructure.Data.EF;
using Product.Infrastructure.MapperProfiles;
using Serilog;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;

namespace Product.Api
{
    public static class Startup
    {
        public static void ConfigureServices(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration)
        {
            AppOptions.ApplicationOptionContext.ConnectionString =
                configuration.GetConnectionString("DefaultConnection");
            
            Console.WriteLine("connectionStrigng =============================" 
                              + configuration.GetConnectionString("DefaultConnection"));
            
            services.AddDbContext<AppDbContext>(opt => 
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    options => options.MigrationsAssembly(Assembly.GetAssembly(typeof(Program))!.GetName().Name))
            );
            
            services.AddDbContext<IntegrationEventLogContext>(options =>
            {
                options.UseSqlServer(configuration["DefaultConnection"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            });

            services.AddAppProblemDetail(environment);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = AppOptions.Jwt.Scheme;
                opt.DefaultScheme = AppOptions.Jwt.Scheme;
                opt.DefaultChallengeScheme = AppOptions.Jwt.Scheme;
            }).AddScheme<AppOptions.Jwt, AppAuthenticationHandler>(AppOptions.Jwt.Scheme, opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                };
            });
            
            services.AddServiceDiscovery(o => o.UseEureka());
            services.AddAppDependencies(configuration);
            services.AddAppOptions(configuration);
            services.AddMemoryCache();
            services.AddGrpc();
            services.AddGrpcClient<IdentityGrpc.IdentityGrpcClient>(o =>
            {
                o.Address = new Uri(configuration["Grpc:Clients:Identity:Url"]);
            }); 
            services.AddControllers();
            services.AddAutoMapper(typeof(Startup), typeof(AutoMapperInfraProfile));
            services.AddAppSwagger();
        }

        public static void Configure(this WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();
            app.UseAppSwagger();
        

            app.UseAppProblemDetail();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            // app.MapGrpcService<AuthGrpcService>();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}