using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Infrastructure.Persistence.Configurations;
internal sealed class LifecycleProgramConfiguration : IEntityTypeConfiguration<LifecycleProgram>
{
    public void Configure(EntityTypeBuilder<LifecycleProgram> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);
    }
}

