
namespace FSH.Framework.Core.Tenant.Abstractions;
public interface ISharedEntity
{
    List<string> SharedWith { get; set; }
}
