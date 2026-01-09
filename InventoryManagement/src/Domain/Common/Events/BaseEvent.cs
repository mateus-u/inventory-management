using Domain.Mediator;

namespace Domain.Common.Events;

public abstract class BaseEvent : INotification
{
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public Guid IdempotencyKey { get; set; } = Guid.NewGuid();
}
