using Application.Common.Services.WMSService.Interface;
using Application.Common.Services.WMSService.Requests;
using Application.Common.Services.WMSService.Responses;
using System.Net.Http.Json;

namespace Infrastructure.Services;

public class WmsService : IWmsService
{
    private readonly HttpClient _httpClient;

    public WmsService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("WmsService");
    }

    public async Task<WmsProductResponse> CreateProductAsync(WmsProductRequest product, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/products", product, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<WmsProductResponse>(cancellationToken);

        return result ?? throw new InvalidOperationException("Failed to deserialize WMS product response");
    }

    public async Task<WmsDispatchResponse> DispatchProductAsync(string wmsProductId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsync($"/products/{wmsProductId}/dispatch", null, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<WmsDispatchResponse>(cancellationToken);

        return result ?? throw new InvalidOperationException("Failed to deserialize WMS dispatch response");
    }
}
