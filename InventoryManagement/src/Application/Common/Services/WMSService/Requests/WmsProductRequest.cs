namespace Application.Common.Services.WMSService.Requests;

public record WmsProductRequest(
    string ProductId,
    string Description,
    string? CategoryShortcode,
    string? SupplierId
);