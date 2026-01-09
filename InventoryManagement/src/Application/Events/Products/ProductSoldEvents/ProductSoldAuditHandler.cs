using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Mediator;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.Events.Products.ProductSoldEvents;

public class ProductSoldAuditHandler : INotificationHandler<ProductSoldEvent>
{
    private readonly ILogger<ProductSoldAuditHandler> _logger;
    private readonly IAuditService _auditService;

    public ProductSoldAuditHandler(
        ILogger<ProductSoldAuditHandler> logger,
        IAuditService auditService)
    {
        _logger = logger;
        _auditService = auditService;
    }

    public async Task HandleAsync(ProductSoldEvent notification, CancellationToken cancellationToken = default)
    {
        try
        {
            var auditLog = new AuditLogDto(
                UserId: "system",
                Email: notification.Product.Supplier.Email.Address,
                ActionName: $"Product Sold: {notification.Product.Description}",
                Timestamp: DateTime.UtcNow
            );

            await _auditService.LogActionAsync(auditLog, cancellationToken);
            
            _logger.LogInformation($"Product sold - Audit logged: {notification.Product.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to log audit for product sale: {notification.Product.Id}");
        }
    }
}
