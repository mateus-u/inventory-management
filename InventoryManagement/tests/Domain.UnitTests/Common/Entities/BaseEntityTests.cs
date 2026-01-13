using Domain.Common.Entities;
using Domain.Common.Events;

namespace Domain.UnitTests.Common.Entities;

public class BaseEntityTests
{
    private class TestEntity : BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    // Concrete event implementation for testing
    private class TestEvent : BaseEvent
    {
    }

    [Fact]
    public void BaseEntity_Constructor_ShouldSetCreatedAtToUtcNow()
    {
        var beforeCreation = DateTime.UtcNow;

        var entity = new TestEntity();

        var afterCreation = DateTime.UtcNow;
        Assert.True(entity.CreatedAt >= beforeCreation && entity.CreatedAt <= afterCreation);
    }

    [Fact]
    public void BaseEntity_Constructor_ShouldInitializeEventsAsEmpty()
    {
        var entity = new TestEntity();

        Assert.Empty(entity.Events);
    }

    [Fact]
    public void AddEvent_ShouldAddEventToEventsCollection()
    {
        var entity = new TestEntity();
        var testEvent = new TestEvent();

        entity.AddEvent(testEvent);

        Assert.Single(entity.Events);
        Assert.Contains(testEvent, entity.Events);
    }

    [Fact]
    public void AddEvent_ShouldAddMultipleEvents()
    {
        var entity = new TestEntity();
        var event1 = new TestEvent();
        var event2 = new TestEvent();
        var event3 = new TestEvent();

        entity.AddEvent(event1);
        entity.AddEvent(event2);
        entity.AddEvent(event3);

        Assert.Equal(3, entity.Events.Count);
        Assert.Contains(event1, entity.Events);
        Assert.Contains(event2, entity.Events);
        Assert.Contains(event3, entity.Events);
    }

    [Fact]
    public void RemoveEvent_ShouldRemoveSpecificEvent()
    {
        var entity = new TestEntity();
        var event1 = new TestEvent();
        var event2 = new TestEvent();
        entity.AddEvent(event1);
        entity.AddEvent(event2);

        entity.RemoveEvent(event1);

        Assert.Single(entity.Events);
        Assert.DoesNotContain(event1, entity.Events);
        Assert.Contains(event2, entity.Events);
    }

    [Fact]
    public void RemoveEvent_WhenEventDoesNotExist_ShouldNotThrowException()
    {
        var entity = new TestEntity();
        var event1 = new TestEvent();
        var event2 = new TestEvent();
        entity.AddEvent(event1);

        var exception = Record.Exception(() => entity.RemoveEvent(event2));

        Assert.Null(exception);
        Assert.Single(entity.Events);
    }

    [Fact]
    public void ClearEvents_ShouldRemoveAllEvents()
    {
        var entity = new TestEntity();
        entity.AddEvent(new TestEvent());
        entity.AddEvent(new TestEvent());
        entity.AddEvent(new TestEvent());

        entity.ClearEvents();

        Assert.Empty(entity.Events);
    }

    [Fact]
    public void ClearEvents_WhenNoEvents_ShouldNotThrowException()
    {
        var entity = new TestEntity();

        var exception = Record.Exception(() => entity.ClearEvents());

        Assert.Null(exception);
        Assert.Empty(entity.Events);
    }

    [Fact]
    public void Events_ShouldReturnReadOnlyCollection()
    {
        var entity = new TestEntity();
        entity.AddEvent(new TestEvent());

        var events = entity.Events;

        Assert.IsAssignableFrom<IReadOnlyCollection<BaseEvent>>(events);
    }

    [Fact]
    public void Events_ModificationAttempt_ShouldNotAffectOriginalCollection()
    {
        var entity = new TestEntity();
        var originalEvent = new TestEvent();
        entity.AddEvent(originalEvent);

        var events = entity.Events;
        var eventsList = events.ToList();
        eventsList.Add(new TestEvent());

        Assert.Single(entity.Events);
        Assert.Equal(2, eventsList.Count);
    }
}
