using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure.Persistence.Configurations;
internal sealed class LifecycleStageConfiguration : IEntityTypeConfiguration<LifecycleStage>
{
    public void Configure(EntityTypeBuilder<LifecycleStage> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);
    }
}

