using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Ranch.Domain.Rations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Persistence.Configurations;
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

