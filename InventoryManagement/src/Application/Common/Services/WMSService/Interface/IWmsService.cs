using Application.Common.Services.WMSService.Requests;
using Application.Common.Services.WMSService.Responses;

namespace Application.Common.Services.WMSService.Interface;

public interface IWmsService
{
    Task<WmsProductResponse> CreateProductAsync(WmsProductRequest product, CancellationToken cancellationToken = default);
    Task<WmsDispatchResponse> DispatchProductAsync(string wmsProductId, CancellationToken cancellationToken = default);
}
