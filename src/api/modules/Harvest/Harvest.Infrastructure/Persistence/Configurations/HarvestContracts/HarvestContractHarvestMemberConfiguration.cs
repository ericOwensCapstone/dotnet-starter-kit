using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts;
using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Persistence.Configurations.HarvestContracts;
internal sealed class HarvestContractHarvestMemberConfiguration : IEntityTypeConfiguration<HarvestContractHarvestMember>
{
    public void Configure(EntityTypeBuilder<HarvestContractHarvestMember> builder)
    {
        builder
            .HasKey(lps => new {lps.HarvestContractId,lps.HarvestMemberId});

        builder
            .HasOne(lps => lps.HarvestMember)
            .WithMany()
            .HasForeignKey(lps => lps.HarvestMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<HarvestContract>()
            .WithMany(lp => lp.HarvestContractHarvestMembers)
            .HasForeignKey(lps => lps.HarvestContractId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Navigation(lps => lps.HarvestMember)
            .AutoInclude();
    }
}
