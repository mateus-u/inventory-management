using Application.Common.DTOs;
using Application.Common.Interfaces;
using System.Net.Http.Json;

namespace Infrastructure.Services;

public class WmsService : IWmsService
{
    private readonly HttpClient _httpClient;

    public WmsService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("WmsService");
    }

    public async Task<WmsProductResponseDto> CreateProductAsync(WmsProductDto product, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/products", product, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<WmsProductResponseDto>(cancellationToken);

        return result ?? throw new InvalidOperationException("Failed to deserialize WMS product response");
    }

    public async Task<WmsDispatchResponseDto> DispatchProductAsync(string wmsProductId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync($"/products/{wmsProductId}/dispatch", null, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<WmsDispatchResponseDto>(cancellationToken);

        return result ?? throw new InvalidOperationException("Failed to deserialize WMS dispatch response");
    }
}
