using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.AnimalTypeCatalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Infrastructure.Persistence.ConfiguanimalTypes;
internal sealed class AnimalTypeConfiguration : IEntityTypeConfiguration<AnimalType>
{
    public void Configure(EntityTypeBuilder<AnimalType> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);
    }
}


