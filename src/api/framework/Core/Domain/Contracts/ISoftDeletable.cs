using Microsoft.EntityFrameworkCore;

namespace FSH.Framework.Core.Domain.Contracts;

public interface ISoftDeletable
{
    DateTimeOffset? Deleted { get; set; }
    Guid? DeletedBy { get; set; }
    (bool CanBeDeleted, string Reason) CanBeSoftDeleted(DbContext context);
}
