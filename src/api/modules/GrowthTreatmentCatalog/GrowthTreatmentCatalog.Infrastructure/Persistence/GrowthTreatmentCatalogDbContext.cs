using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedDbContextProject;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Infrastructure.Persistence;

public sealed class GrowthTreatmentCatalogDbContext : SharedDbContext
{
    public GrowthTreatmentCatalogDbContext(IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor, DbContextOptions<GrowthTreatmentCatalogDbContext> options, IPublisher publisher, IOptions<DatabaseOptions> settings)
        : base(multiTenantContextAccessor, options, publisher, settings)
    {
    }

    //public DbSet<GrowthTreatment> GrowthTreatments { get; set; } = null!;

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    ArgumentNullException.ThrowIfNull(modelBuilder);
    //    modelBuilder.ApplyConfigurationsFromAssembly(typeof(GrowthTreatmentCatalogDbContext).Assembly);
    //    modelBuilder.HasDefaultSchema(SchemaNames.GrowthTreatmentCatalog);
    //}
}

