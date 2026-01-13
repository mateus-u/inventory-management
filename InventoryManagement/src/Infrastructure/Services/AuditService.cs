using Application.Common.Services.AuditService.Interface;
using Application.Common.Services.AuditService.Requests;
using Application.Common.Services.AuditService.Responses;
using System.Net.Http.Json;

namespace Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly HttpClient _httpClient;

    public AuditService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("AuditService");
    }

    public async Task<AuditLogResponse> LogActionAsync(AuditLogRequest auditLog, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("/logs", auditLog, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AuditLogResponse>(cancellationToken);

        return result ?? throw new InvalidOperationException("Failed to deserialize audit log response");
    }
}
