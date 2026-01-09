using Application.Common.DTOs;

namespace Application.Common.Interfaces;

public interface IWmsService
{
    Task<WmsProductResponseDto> CreateProductAsync(WmsProductDto product, CancellationToken cancellationToken = default);
    Task<WmsDispatchResponseDto> DispatchProductAsync(string wmsProductId, CancellationToken cancellationToken = default);
}
