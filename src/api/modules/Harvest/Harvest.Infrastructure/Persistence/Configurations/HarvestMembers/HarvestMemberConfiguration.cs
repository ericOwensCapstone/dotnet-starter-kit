using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Persistence.Configurations.HarvestMembers;
internal sealed class HarvestMemberConfiguration : IEntityTypeConfiguration<HarvestMember>
{
    public void Configure(EntityTypeBuilder<HarvestMember> builder)
    {
        builder.Property<DateTimeOffset?>("Deleted"); // Define Deleted as a shadow property  
        builder.HasIndex("Name", "TenantId").IsUnique().HasFilter("\"Deleted\" IS NULL");
        builder.HasIndex("TenantId");
        builder.HasIndex("MemberId");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(99);
        builder.Property(x => x.Description).HasMaxLength(999);        builder.HasIndex("TenantId").IsUnique().HasFilter("\"Deleted\" IS NULL");

    }
}