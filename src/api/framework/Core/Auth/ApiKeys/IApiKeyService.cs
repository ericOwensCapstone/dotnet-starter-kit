namespace FSH.Framework.Core.Auth.ApiKeys;

public interface IApiKeyService
{
    Task<ApiKeyValidationResult> ValidateKeyAsync(string apiKey, CancellationToken cancellationToken = default);
    Task<string> CreateApiKeyAsync(CreateApiKeyRequest request, CancellationToken cancellationToken = default);
    Task DeactivateApiKeyAsync(Guid keyId, CancellationToken cancellationToken = default);
    Task<List<ApiKeyDto>> GetApiKeysAsync(string tenantId, CancellationToken cancellationToken = default);
}

public class ApiKeyValidationResult
{
    public bool IsValid { get; set; }
    public string? TenantId { get; set; }
    public string? KeyName { get; set; }
    public string? FailureReason { get; set; }
}

public class CreateApiKeyRequest
{
    public string Name { get; set; } = default!;
    public string TenantId { get; set; } = default!;
    public DateTime? ExpiryDate { get; set; }
}

public class ApiKeyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string TenantId { get; set; } = default!;
    public DateTimeOffset Created { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime LastUsedAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsExpired { get; set; }
}