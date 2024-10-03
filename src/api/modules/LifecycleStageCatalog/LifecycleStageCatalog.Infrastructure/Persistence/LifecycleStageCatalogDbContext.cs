using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure.Persistence;

public sealed class LifecycleStageCatalogDbContext : FshDbContext
{
    public LifecycleStageCatalogDbContext(IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor, DbContextOptions<LifecycleStageCatalogDbContext> options, IPublisher publisher, IOptions<DatabaseOptions> settings)
        : base(multiTenantContextAccessor, options, publisher, settings)
    {
    }

    public DbSet<LifecycleStage> LifecycleStages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LifecycleStageCatalogDbContext).Assembly);
        modelBuilder.HasDefaultSchema(SchemaNames.LifecycleStageCatalog);
    }
}

