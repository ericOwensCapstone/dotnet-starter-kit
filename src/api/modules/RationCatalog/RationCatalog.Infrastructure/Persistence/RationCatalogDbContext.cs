using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.WebApi.RationCatalog.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedDbContextProject;

namespace FSH.Starter.WebApi.RationCatalog.Infrastructure.Persistence;

public sealed class RationCatalogDbContext : SharedDbContext
{
    public RationCatalogDbContext(IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor, DbContextOptions<RationCatalogDbContext> options, IPublisher publisher, IOptions<DatabaseOptions> settings)
        : base(multiTenantContextAccessor, options, publisher, settings)
    {
    }

    //public DbSet<Ration> Rations { get; set; } = null!;

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    ArgumentNullException.ThrowIfNull(modelBuilder);
    //    modelBuilder.ApplyConfigurationsFromAssembly(typeof(RationCatalogDbContext).Assembly);
    //    modelBuilder.HasDefaultSchema(SchemaNames.RationCatalog);
    //}
}

