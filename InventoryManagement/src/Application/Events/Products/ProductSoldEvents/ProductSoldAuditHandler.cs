using Application.Common.Interfaces;
using Application.Common.Mediator;
using Application.Common.Services.AuditService.Interface;
using Application.Common.Services.AuditService.Requests;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.Events.Products.ProductSoldEvents;

public class ProductSoldAuditHandler : INotificationHandler<ProductSoldEvent>
{
    private readonly ILogger<ProductSoldAuditHandler> _logger;
    private readonly IAuditService _auditService;
    private readonly ICurrentUser _currentUser;

    public ProductSoldAuditHandler(
        ILogger<ProductSoldAuditHandler> logger,
        IAuditService auditService,
        ICurrentUser currentUser)
    {
        _logger = logger;
        _auditService = auditService;
        _currentUser = currentUser;
    }

    public async Task HandleAsync(ProductSoldEvent notification, CancellationToken cancellationToken = default)
    {
        try
        {
            var auditLog = new AuditLogRequest(
                UserId: _currentUser.Id.ToString(),
                Email: _currentUser.Email,
                ActionName: $"product-sold",
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
