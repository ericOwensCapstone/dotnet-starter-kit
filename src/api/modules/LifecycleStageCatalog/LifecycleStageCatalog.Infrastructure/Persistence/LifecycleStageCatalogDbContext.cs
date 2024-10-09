using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using FSH.Starter.WebApi.RationCatalog.Domain;
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
    public DbSet<Ration> Rations { get; set; } = null!;
    public DbSet<GrowthTreatment> GrowthTreatments { get; set; } = null!;
    public DbSet<PreventativeTreatment> PreventativeTreatments { get; set; } = null!;

    public DbSet<LifecycleStage> LifecycleStages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LifecycleStageCatalogDbContext).Assembly);
        modelBuilder.HasDefaultSchema(SchemaNames.LifecycleStageCatalog);

        modelBuilder.Entity<Ration>()
            .ToTable("Rations", schema: "rationcatalog")
            .Metadata.SetIsTableExcludedFromMigrations(true);

        modelBuilder.Entity<GrowthTreatment>()
            .ToTable("GrowthTreatments", schema: "growthtreatmentcatalog")
            .Metadata.SetIsTableExcludedFromMigrations(true);

        modelBuilder.Entity<PreventativeTreatment>()
            .ToTable("PreventativeTreatments", schema: "preventativetreatmentcatalog")
            .Metadata.SetIsTableExcludedFromMigrations(true);

        modelBuilder.Entity<LifecycleStage>()
            .ToTable("LifecycleStages", schema: "lifecyclestagecatalog");

        modelBuilder.Entity<LifecycleStage>()
            .Property<Guid>("LifecycleProgramId");
    }
}

