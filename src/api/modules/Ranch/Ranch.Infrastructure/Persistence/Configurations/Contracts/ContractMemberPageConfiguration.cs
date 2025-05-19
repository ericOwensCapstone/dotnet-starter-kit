using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Ranch.Domain.Contracts;
using FSH.Starter.WebApi.Ranch.Domain.MemberPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Persistence.Configurations.Contracts;
internal sealed class ContractMemberPageConfiguration : IEntityTypeConfiguration<ContractMemberPage>
{
    public void Configure(EntityTypeBuilder<ContractMemberPage> builder)
    {
        builder
            .HasKey(lps => new {lps.ContractId,lps.MemberPageId});

        builder
            .HasOne(lps => lps.MemberPage)
            .WithMany()
            .HasForeignKey(lps => lps.MemberPageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<Contract>()
            .WithMany(lp => lp.ContractMemberPages)
            .HasForeignKey(lps => lps.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Navigation(lps => lps.MemberPage)
            .AutoInclude();
    }
}
