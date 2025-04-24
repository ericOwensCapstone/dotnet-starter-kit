using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Ranch.Domain.Rations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Persistence.Configurations.Rations;
internal sealed class RationConfiguration : IEntityTypeConfiguration<Ration>
{
    public void Configure(EntityTypeBuilder<Ration> builder)
    {
        builder.Property<DateTimeOffset?>("Deleted"); // Define Deleted as a shadow property  
        builder.HasIndex("Name", "TenantId").IsUnique().HasFilter("\"Deleted\" IS NULL");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(99);
        builder.Property(x => x.Description).HasMaxLength(999);
    }
}
