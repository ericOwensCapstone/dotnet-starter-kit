using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Ranch.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Persistence.Configurations.Contracts;
internal sealed class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.Property<DateTimeOffset?>("Deleted"); // Define Deleted as a shadow property  
        builder.HasIndex("Name", "TenantId").IsUnique().HasFilter("\"Deleted\" IS NULL");
        builder.HasIndex("TenantId");
        builder.HasIndex("MemberId");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(99);
        builder.Property(x => x.Description).HasMaxLength(999);
        builder.HasOne(l => l.ContractStatus)
               .WithMany()
               .HasForeignKey(l => l.ContractStatusId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(ls => ls.ContractStatus).AutoInclude();
        builder.Navigation(lp => lp.ContractMemberPages).AutoInclude();

    }
}