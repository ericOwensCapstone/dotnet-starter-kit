
namespace FSH.Framework.Core.Tenant.Abstractions;
public interface ITenantEntity
{
    string? TenantId { get; set; }
    Guid? MemberId { get; set; }
}
