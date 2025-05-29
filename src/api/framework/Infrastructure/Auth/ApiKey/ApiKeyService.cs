using System.Security.Cryptography;
using Ardalis.Specification;
using FSH.Framework.Core.Auth.ApiKeys;
using FSH.Framework.Core.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Framework.Infrastructure.Auth.ApiKey;

public class ApiKeyService : IApiKeyService
{
    private readonly IRepository<Core.Auth.ApiKeys.ApiKey> _repository;
    private readonly ILogger<ApiKeyService> _logger;

    public ApiKeyService([FromKeyedServices("identity:apikeys")] IRepository<Core.Auth.ApiKeys.ApiKey> repository, ILogger<ApiKeyService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<ApiKeyValidationResult> ValidateKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var keyHash = HashApiKey(apiKey);
            
            var specification = new ApiKeyByHashSpec(keyHash);
            var apiKeyEntity = await _repository.SingleOrDefaultAsync(specification, cancellationToken);

            if (apiKeyEntity == null)
            {
                return new ApiKeyValidationResult 
                { 
                    IsValid = false, 
                    FailureReason = "API key not found" 
                };
            }

            if (!apiKeyEntity.IsActive)
            {
                return new ApiKeyValidationResult 
                { 
                    IsValid = false, 
                    FailureReason = "API key is inactive" 
                };
            }

            if (apiKeyEntity.IsExpired())
            {
                return new ApiKeyValidationResult 
                { 
                    IsValid = false, 
                    FailureReason = "API key has expired" 
                };
            }

            // Update last used timestamp
            apiKeyEntity.UpdateLastUsed();
            await _repository.UpdateAsync(apiKeyEntity, cancellationToken);

            return new ApiKeyValidationResult
            {
                IsValid = true,
                TenantId = apiKeyEntity.TenantId,
                KeyName = apiKeyEntity.Name
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating API key");
            return new ApiKeyValidationResult 
            { 
                IsValid = false, 
                FailureReason = "Error validating API key" 
            };
        }
    }

    public async Task<string> CreateApiKeyAsync(CreateApiKeyRequest request, CancellationToken cancellationToken = default)
    {
        var apiKey = GenerateApiKey();
        var keyHash = HashApiKey(apiKey);

        var apiKeyEntity = Core.Auth.ApiKeys.ApiKey.Create(
            request.Name,
            keyHash,
            request.TenantId,
            request.ExpiryDate);

        await _repository.AddAsync(apiKeyEntity, cancellationToken);

        // Return the actual API key only once during creation
        return apiKey;
    }

    public async Task DeactivateApiKeyAsync(Guid keyId, CancellationToken cancellationToken = default)
    {
        var apiKey = await _repository.GetByIdAsync(keyId, cancellationToken);
        if (apiKey == null)
        {
            throw new NotFoundException($"API Key with ID {keyId} not found");
        }

        apiKey.Deactivate();
        await _repository.UpdateAsync(apiKey, cancellationToken);
    }

    public async Task<List<ApiKeyDto>> GetApiKeysAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        var specification = new ApiKeysByTenantSpec(tenantId);
        var apiKeys = await _repository.ListAsync(specification, cancellationToken);

        return apiKeys.Select(k => new ApiKeyDto
        {
            Id = k.Id,
            Name = k.Name,
            TenantId = k.TenantId,
            Created = k.Created,
            ExpiryDate = k.ExpiryDate,
            LastUsedAt = k.LastUsedAt,
            IsActive = k.IsActive,
            IsExpired = k.IsExpired()
        }).ToList();
    }

    private static string GenerateApiKey()
    {
        const int keyLength = 32;
        var randomBytes = new byte[keyLength];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    private static string HashApiKey(string apiKey)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(apiKey));
        return Convert.ToBase64String(hashBytes);
    }
}

public class ApiKeyByHashSpec : Specification<Core.Auth.ApiKeys.ApiKey>, ISingleResultSpecification<Core.Auth.ApiKeys.ApiKey>
{
    public ApiKeyByHashSpec(string keyHash)
    {
        Query.Where(k => k.KeyHash == keyHash);
    }
}

public class ApiKeysByTenantSpec : Specification<Core.Auth.ApiKeys.ApiKey>
{
    public ApiKeysByTenantSpec(string tenantId)
    {
        Query.Where(k => k.TenantId == tenantId)
             .OrderByDescending(k => k.Created);
    }
}