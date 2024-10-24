using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.WeatherZoneCatalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Infrastructure.Persistence.ConfiguweatherZones;
internal sealed class WeatherZoneConfiguration : IEntityTypeConfiguration<WeatherZone>
{
    public void Configure(EntityTypeBuilder<WeatherZone> builder)
    {
        builder.IsMultiTenant();
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);
    }
}


