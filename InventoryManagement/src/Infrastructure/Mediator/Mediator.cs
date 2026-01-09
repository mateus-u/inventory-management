using Application.Common.Mediator;
using Domain.Mediator;

namespace Infrastructure.Mediator;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        var handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
        {
            throw new InvalidOperationException($"No handler found for request type {requestType.FullName}");
        }

        var method = handlerType.GetMethod("HandleAsync");

        if (method == null)
        {
            throw new InvalidOperationException($"No HandleAsync method found in handler for request type {requestType.FullName}");
        }

        return (Task<TResponse>)method.Invoke(handler, [request, cancellationToken])!;
    }

    public async Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        var notificationType = notification.GetType();
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);

        var handlers = _serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(handlerType));

        if (handlers == null)
        {
            return;
        }

        var method = handlerType.GetMethod("HandleAsync");

        if (method == null)
        {
            throw new InvalidOperationException($"No HandleAsync method found in handler for notification type {notificationType.FullName}");
        }

        foreach (var handler in (IEnumerable<object>)handlers)
        {
            await ((Task)method.Invoke(handler, [notification, cancellationToken])!).ConfigureAwait(false);
        }
    }
}