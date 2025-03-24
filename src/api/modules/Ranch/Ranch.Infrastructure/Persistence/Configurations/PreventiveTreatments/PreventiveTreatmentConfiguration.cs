using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Persistence.Configurations.PreventiveTreatments;
internal sealed class PreventiveTreatmentConfiguration : IEntityTypeConfiguration<PreventiveTreatment>
{
    public void Configure(EntityTypeBuilder<PreventiveTreatment> builder)
    {
        builder.IsMultiTenant();
        builder.Property<string>("TenantId"); // Define TenantId as a shadow property
        builder.Property<DateTimeOffset?>("Deleted"); // Define Deleted as a shadow property  
        builder.HasIndex("Name", "TenantId").IsUnique().HasFilter("\"Deleted\" IS NULL");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(99);
        builder.Property(x => x.Description).HasMaxLength(999);
    }
}
