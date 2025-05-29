using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;

namespace FSH.Framework.Core.Auth.ApiKeys;

public class ApiKey : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string KeyHash { get; private set; } = default!;
    public string TenantId { get; private set; } = default!;
    public DateTime? ExpiryDate { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime LastUsedAt { get; private set; }
    
    public static ApiKey Create(string name, string keyHash, string tenantId, DateTime? expiryDate = null)
    {
        return new ApiKey
        {
            Name = name,
            KeyHash = keyHash,
            TenantId = tenantId,
            ExpiryDate = expiryDate,
            IsActive = true,
            LastUsedAt = DateTime.UtcNow,
            Created = DateTimeOffset.UtcNow
        };
    }
    
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    public void UpdateLastUsed() => LastUsedAt = DateTime.UtcNow;
    
    public bool IsExpired() => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
}