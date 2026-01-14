using Application.Common.Interfaces;
using Application.Common.Mediator;
using Application.Common.Services.AuditService.Interface;
using Application.Common.Services.AuditService.Requests;
using Domain.Events;
using Microsoft.Extensions.Logging;
namespace Application.Events.Products.ProductCreatedEvents;

public class ProductCreatedAuditHandler : INotificationHandler<ProductCreatedEvent>
{
    private readonly ILogger<ProductCreatedAuditHandler> _logger;
    private readonly IAuditService _auditService;
    private readonly ICurrentUser _currentUser;
    public ProductCreatedAuditHandler(
        ILogger<ProductCreatedAuditHandler> logger,
        IAuditService auditService,
        ICurrentUser currentUser)
    {
        _logger = logger;
        _auditService = auditService;
        _currentUser = currentUser;
    }

    public async Task HandleAsync(ProductCreatedEvent notification, CancellationToken cancellationToken = default)
    {
        try
        {
            var auditLog = new AuditLogRequest(
                UserId: _currentUser.Id.ToString()!,
                Email: _currentUser.Email.ToString()!,
                ActionName: $"product-created",
                Timestamp: DateTime.UtcNow
            );

            var response = await _auditService.LogActionAsync(auditLog, cancellationToken);

            _logger.LogInformation("Audit logged for product creation: {ProductId}, Message: {Message}", notification.Product.Id, response.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log audit for product creation: {ProductId}", notification.Product.Id);
        }
    }
}
