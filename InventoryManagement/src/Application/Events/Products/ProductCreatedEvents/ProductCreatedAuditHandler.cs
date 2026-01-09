using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Mediator;
using Domain.Events;
using Microsoft.Extensions.Logging;
namespace Application.Events.Products.ProductCreatedEvents;

public class ProductCreatedAuditHandler : INotificationHandler<ProductCreatedEvent>
{
    private readonly ILogger<ProductCreatedAuditHandler> _logger;
    private readonly IAuditService _auditService;

    public ProductCreatedAuditHandler(
        ILogger<ProductCreatedAuditHandler> logger,
        IAuditService auditService)
    {
        _logger = logger;
        _auditService = auditService;
    }

    public async Task HandleAsync(ProductCreatedEvent notification, CancellationToken cancellationToken = default)
    {
        try
        {
            var auditLog = new AuditLogDto(
                UserId: "system",
                Email: notification.Product.Supplier.Email.Address,
                ActionName: $"Product Created: {notification.Product.Description}",
                Timestamp: DateTime.UtcNow
            );

            await _auditService.LogActionAsync(auditLog, cancellationToken);
            
            _logger.LogInformation($"Product created - Audit logged: {notification.Product.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to log audit for product creation: {notification.Product.Id}");
        }
    }
}
