using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.RationCatalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.RationCatalog.Infrastructure.Persistence.Configurations;
internal sealed class RationConfiguration : IEntityTypeConfiguration<Ration>
{
    public void Configure(EntityTypeBuilder<Ration> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);
    }
}

