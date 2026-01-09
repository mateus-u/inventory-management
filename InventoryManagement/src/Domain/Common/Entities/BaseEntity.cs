using Domain.Common.Events;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Common.Entities;

public class BaseEntity
{
    public DateTime CreatedAt { get; private set; }

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> Events => _events.ToList().AsReadOnly();

    public BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }

    private ICollection<BaseEvent> _events { get; } = new List<BaseEvent>();

    public void AddEvent(BaseEvent @event)
    {
        _events.Add(@event);
    }

    public void RemoveEvent(BaseEvent @event)
    {
        _events.Remove(@event);
    }

    public void ClearEvents()
    {
        _events.Clear();
    }


}
