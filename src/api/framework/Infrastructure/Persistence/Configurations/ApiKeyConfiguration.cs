using FSH.Framework.Core.Auth.ApiKeys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Constants;

namespace FSH.Framework.Infrastructure.Persistence.Configurations;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.ToTable("ApiKeys", SchemaNames.Identity);

        builder.Property(k => k.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(k => k.KeyHash)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(k => k.TenantId)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(k => k.KeyHash)
            .IsUnique();

        builder.HasIndex(k => k.TenantId);
    }
}