using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Infrastructure.Persistence.Configurations;
internal sealed class GrowthTreatmentConfiguration : IEntityTypeConfiguration<GrowthTreatment>
{
    public void Configure(EntityTypeBuilder<GrowthTreatment> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);
    }
}

