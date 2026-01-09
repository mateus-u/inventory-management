using Application.Common.Interfaces;
using Application.Common.Mediator;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediator(typeof(IApplicationDbContext).Assembly);
        services.AddValidatorsFromAssembly(typeof(IApplicationDbContext).Assembly);

        return services;
    }
    public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
    {
        var handlerType = typeof(IHandler<,>);
        var notificationHandlerType = typeof(INotificationHandler<>);

        foreach (var assembly in assemblies)
        {
            var handlerTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType)
                    .Select(i => new { HandlerType = t, InterfaceType = i }));

            foreach (var handler in handlerTypes)
            {
                services.AddTransient(handler.InterfaceType, handler.HandlerType);
            }

            var notificationHandlerTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == notificationHandlerType)
                    .Select(i => new { HandlerType = t, InterfaceType = i }));

            foreach (var handler in notificationHandlerTypes)
            {
                services.AddTransient(handler.InterfaceType, handler.HandlerType);
            }
        }

        return services;
    }
}
