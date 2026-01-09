using Application.Common.Interfaces;
using Application.Common.Mediator;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.Events.Products.ProductSoldEvents;

public class ProductSoldEmailHandler : INotificationHandler<ProductSoldEvent>
{
    private readonly ILogger<ProductSoldEmailHandler> _logger;
    private readonly IEmailService _emailService;

    public ProductSoldEmailHandler(ILogger<ProductSoldEmailHandler> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task HandleAsync(ProductSoldEvent notification, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = notification.Product;
            var subject = $"Product Sold - {product.Description}";
            var body = $@"
                <html>
                <body>
                    <h2>Product Sold Notification</h2>
                    <p>A product has been sold successfully.</p>
                    <h3>Product Details:</h3>
                    <ul>
                        <li><strong>ID:</strong> {product.Id}</li>
                        <li><strong>Description:</strong> {product.Description}</li>
                        <li><strong>Supplier:</strong> {product.Supplier.Name}</li>
                        <li><strong>Category:</strong> {product.Category.Name}</li>
                        <li><strong>Sold Date:</strong> {product.SoldDate:yyyy-MM-dd HH:mm:ss}</li>
                    </ul>
                </body>
                </html>";

            await _emailService.SendEmailAsync(
                product.Supplier.Email.Address,
                subject,
                body,
                isHtml: true,
                cancellationToken);

            _logger.LogInformation("Product sold - Email sent to {Email} for product {ProductId}", 
                product.Supplier.Email.Address, product.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email notification for sold product {ProductId}", 
                notification.Product.Id);
            // Don't rethrow - we don't want email failures to break the main flow
        }
    }
}
