using Application.Common.Interfaces;
using Application.Common.Mediator;
using Application.Common.Services.AuditService.Interface;
using Application.Common.Services.WMSService.Interface;
using Infrastructure.Database;
using Infrastructure.Database.Interceptors;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddScoped<ISaveChangesInterceptor, DomainEventInterceptor>();

        services.AddDbContext<ApplicationDbContext>((provider, options) =>
            options.UseNpgsql(connectionString)
                .AddInterceptors(provider.GetServices<ISaveChangesInterceptor>())
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
              );

        services.AddTransient<IMediator, Mediator.Mediator>();

        services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>()!);

        services.AddHttpClient("AuditService", client =>
        {
            client.BaseAddress = new Uri(configuration["ExternalServices:AuditService:BaseUrl"] ?? "http://localhost:1081");
        });

        services.AddHttpClient("WmsService", client =>
        {
            client.BaseAddress = new Uri(configuration["ExternalServices:WmsService:BaseUrl"] ?? "http://localhost:1080");
        });

        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IWmsService, WmsService>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}