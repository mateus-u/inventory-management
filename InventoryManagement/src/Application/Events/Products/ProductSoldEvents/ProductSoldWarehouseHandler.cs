using Application.Common.Mediator;
using Application.Common.Services.WMSService.Interface;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.Events.Products.ProductSoldEvents;

public class ProductSoldWarehouseHandler : INotificationHandler<ProductSoldEvent>
{
    private readonly ILogger<ProductSoldWarehouseHandler> _logger;
    private readonly IWmsService _wmsService;

    public ProductSoldWarehouseHandler(
        ILogger<ProductSoldWarehouseHandler> logger,
        IWmsService wmsService)
    {
        _logger = logger;
        _wmsService = wmsService;
    }

    public async Task HandleAsync(ProductSoldEvent notification, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(notification.Product.WmsProductId))
            {
                _logger.LogWarning($"Product {notification.Product.Id} has no WmsProductId, cannot dispatch");
                return;
            }

            var response = await _wmsService.DispatchProductAsync(notification.Product.WmsProductId, cancellationToken);

            _logger.LogInformation("Dispatch WMS product: {ProductId}, Message: {Message}", notification.Product.Id, response.Message);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to dispatch product in WMS: {ProductId}", notification.Product.Id);
        }
    }
}
