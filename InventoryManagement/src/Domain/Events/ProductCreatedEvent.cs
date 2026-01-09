using Domain.Common.Events;
using Domain.Entities;

namespace Domain.Events;

public class ProductCreatedEvent : BaseEvent
{
    public Product Product { get; private set; }

    public ProductCreatedEvent(Product product)
    {
        Product = product;
    }
}
