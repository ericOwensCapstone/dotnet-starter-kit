using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Persistence.Configurations.HarvestContracts;
internal sealed class HarvestContractConfiguration : IEntityTypeConfiguration<HarvestContract>
{
    public void Configure(EntityTypeBuilder<HarvestContract> builder)
    {
        builder.Property<DateTimeOffset?>("Deleted"); // Define Deleted as a shadow property  
        builder.HasIndex("Name", "TenantId").IsUnique().HasFilter("\"Deleted\" IS NULL");
        builder.HasIndex("TenantId");
        builder.HasIndex("MemberId");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(99);
        builder.Property(x => x.Description).HasMaxLength(999);
        builder.HasOne(l => l.HarvestContractStatus)
               .WithMany()
               .HasForeignKey(l => l.HarvestContractStatusId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(ls => ls.HarvestContractStatus).AutoInclude();
        builder.Navigation(lp => lp.HarvestContractHarvestMembers).AutoInclude();

    }
}