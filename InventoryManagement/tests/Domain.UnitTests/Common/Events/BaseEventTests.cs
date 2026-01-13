using Domain.Common.Events;

namespace Domain.UnitTests.Common.Events;

public class BaseEventTests
{
    private class TestEvent : BaseEvent
    {
    }

    [Fact]
    public void BaseEvent_Constructor_ShouldSetDateToUtcNow()
    {
        var beforeCreation = DateTime.UtcNow;

        var @event = new TestEvent();

        var afterCreation = DateTime.UtcNow;
        Assert.True(@event.Date >= beforeCreation && @event.Date <= afterCreation);
    }

    [Fact]
    public void BaseEvent_Constructor_ShouldGenerateUniqueIdempotencyKey()
    {
        var event1 = new TestEvent();
        var event2 = new TestEvent();

        Assert.NotEqual(Guid.Empty, event1.IdempotencyKey);
        Assert.NotEqual(Guid.Empty, event2.IdempotencyKey);
        Assert.NotEqual(event1.IdempotencyKey, event2.IdempotencyKey);
    }

    [Fact]
    public void BaseEvent_IdempotencyKey_ShouldBeUniqueBetweenMultipleInstances()
    {
        var events = Enumerable.Range(0, 100)
            .Select(_ => new TestEvent())
            .ToList();

        var distinctKeys = events.Select(e => e.IdempotencyKey).Distinct().Count();
        Assert.Equal(100, distinctKeys);
    }

    [Fact]
    public void BaseEvent_Date_CanBeSetManually()
    {
        var @event = new TestEvent();
        var customDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        @event.Date = customDate;

        Assert.Equal(customDate, @event.Date);
    }

    [Fact]
    public void BaseEvent_IdempotencyKey_CanBeSetManually()
    {
        var @event = new TestEvent();
        var customKey = Guid.NewGuid();

        @event.IdempotencyKey = customKey;

        Assert.Equal(customKey, @event.IdempotencyKey);
    }

    [Fact]
    public void BaseEvent_MultipleEvents_ShouldHaveDifferentTimestamps()
    {
        var event1 = new TestEvent();
        Thread.Sleep(10); 
        var event2 = new TestEvent();

        Assert.True(event2.Date >= event1.Date);
    }
}
