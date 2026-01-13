using Application.Common.Interfaces;
using Application.Common.Mediator;
using Application.Common.Services.WMSService.Interface;
using Application.Common.Services.WMSService.Requests;
using Domain.Events;
using Microsoft.Extensions.Logging;
namespace Application.Events.Products.ProductCreatedEvents;

public class ProductCreatedWarehouseHandler : INotificationHandler<ProductCreatedEvent>
{
    private readonly ILogger<ProductCreatedWarehouseHandler> _logger;
    private readonly IWmsService _wmsService;
    private readonly IApplicationDbContext _dbContext;

    public ProductCreatedWarehouseHandler(
        ILogger<ProductCreatedWarehouseHandler> logger,
        IWmsService wmsService,
        IApplicationDbContext dbContext)
    {
        _logger = logger;
        _wmsService = wmsService;
        _dbContext = dbContext;
    }

    public async Task HandleAsync(ProductCreatedEvent notification, CancellationToken cancellationToken = default)
    {
        try
        {
            var wmsProduct = new WmsProductRequest(
                ProductId: notification.Product.Id.ToString(),
                Description: notification.Product.Description,
                CategoryShortcode: notification.Product.Category?.Shortcode,
                SupplierId: notification.Product.Supplier?.Id.ToString()
            );

            var response = await _wmsService.CreateProductAsync(wmsProduct, cancellationToken);
            
            notification.Product.SetWmsProductId(response.WmsProductId);
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation($"Product created in WMS: {notification.Product.Id} -> {response.WmsProductId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to create product in WMS: {notification.Product.Id}");
        }
    }
}
