using FSH.Framework.Core.Domain.Contracts;
using Microsoft.EntityFrameworkCore;

namespace FSH.Framework.Core.Domain;

public class AuditableEntity<TId> : BaseEntity<TId>, IAuditable, ISoftDeletable
{
    public DateTimeOffset Created { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset LastModified { get; set; }
    public Guid? LastModifiedBy { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public Guid? DeletedBy { get; set; }

    public virtual (bool CanBeDeleted, string Reason) CanBeSoftDeleted(DbContext context)
    {
        return (true, string.Empty);
    }
}

public abstract class AuditableEntity : AuditableEntity<Guid>
{
    protected AuditableEntity() => Id = Guid.NewGuid();
}
