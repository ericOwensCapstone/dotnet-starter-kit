using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Members.Domain.MemberAds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Members.Infrastructure.Persistence.Configurations.MemberAds;
internal sealed class MemberAdConfiguration : IEntityTypeConfiguration<MemberAd>
{
    public void Configure(EntityTypeBuilder<MemberAd> builder)
    {
        builder.Property<DateTimeOffset?>("Deleted"); // Define Deleted as a shadow property  
        builder.HasIndex("Name", "TenantId").IsUnique().HasFilter("\"Deleted\" IS NULL");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(99);
        builder.Property(x => x.Description).HasMaxLength(999);
    }
}