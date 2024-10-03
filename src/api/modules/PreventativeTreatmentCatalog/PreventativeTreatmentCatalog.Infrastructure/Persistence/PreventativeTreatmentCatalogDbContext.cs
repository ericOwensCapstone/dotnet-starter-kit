using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Infrastructure.Persistence;

public sealed class PreventativeTreatmentCatalogDbContext : FshDbContext
{
    public PreventativeTreatmentCatalogDbContext(IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor, DbContextOptions<PreventativeTreatmentCatalogDbContext> options, IPublisher publisher, IOptions<DatabaseOptions> settings)
        : base(multiTenantContextAccessor, options, publisher, settings)
    {
    }

    public DbSet<PreventativeTreatment> PreventativeTreatments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PreventativeTreatmentCatalogDbContext).Assembly);
        modelBuilder.HasDefaultSchema(SchemaNames.PreventativeTreatmentCatalog);
    }
}

