
namespace FSH.Framework.Core.Tenant.Abstractions;
public interface IShareableEntity
{
    List<Guid> SharedWith { get; set; }
}
