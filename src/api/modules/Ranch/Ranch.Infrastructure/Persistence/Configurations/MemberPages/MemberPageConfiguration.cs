using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Ranch.Domain.MemberPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Persistence.Configurations.MemberPages;
internal sealed class MemberPageConfiguration : IEntityTypeConfiguration<MemberPage>
{
    public void Configure(EntityTypeBuilder<MemberPage> builder)
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