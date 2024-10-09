using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Infrastructure.Persistence;

public sealed class LifecycleProgramCatalogDbContext : FshDbContext
{
    public LifecycleProgramCatalogDbContext(IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor, DbContextOptions<LifecycleProgramCatalogDbContext> options, IPublisher publisher, IOptions<DatabaseOptions> settings)
        : base(multiTenantContextAccessor, options, publisher, settings)
    {
    }

    public DbSet<LifecycleProgram> LifecyclePrograms { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LifecycleProgramCatalogDbContext).Assembly);
        modelBuilder.HasDefaultSchema(SchemaNames.LifecycleProgramCatalog);
    }
}

