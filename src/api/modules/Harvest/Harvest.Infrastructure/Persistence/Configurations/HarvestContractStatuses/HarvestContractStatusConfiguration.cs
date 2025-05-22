using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Persistence.Configurations.HarvestContractStatuses;
internal sealed class HarvestContractStatusConfiguration : IEntityTypeConfiguration<HarvestContractStatus>
{
    public void Configure(EntityTypeBuilder<HarvestContractStatus> builder)
    {
        builder.Property<DateTimeOffset?>("Deleted"); // Define Deleted as a shadow property  
        builder.HasIndex("Name", "TenantId").IsUnique().HasFilter("\"Deleted\" IS NULL");
        builder.HasIndex("TenantId");
        builder.HasIndex("MemberId");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(99);
        builder.Property(x => x.Description).HasMaxLength(999);
    }
}