using Domain.Common.Events;
using Domain.Entities;

namespace Domain.Events;

public class ProductSoldEvent: BaseEvent
{
    public Product Product { get; private set; }

    public ProductSoldEvent(Product product)
    {
        Product = product;
    }
}
