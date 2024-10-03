using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Infrastructure.Persistence.Configurations;
internal sealed class PreventativeTreatmentConfiguration : IEntityTypeConfiguration<PreventativeTreatment>
{
    public void Configure(EntityTypeBuilder<PreventativeTreatment> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);
    }
}

