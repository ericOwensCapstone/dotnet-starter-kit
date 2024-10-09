using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using FSH.Starter.WebApi.RationCatalog.Domain;
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
    public DbSet<LifecycleStage> LifecycleStages { get; set; } = null;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LifecycleProgramCatalogDbContext).Assembly);
        modelBuilder.HasDefaultSchema(SchemaNames.LifecycleProgramCatalog);

        modelBuilder.Entity<LifecycleStage>()
            .ToTable("LifecycleStages", schema: "lifecyclestagecatalog")
            .Metadata.SetIsTableExcludedFromMigrations(true);

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
            .Property<Guid>("LifecycleProgramId");

        modelBuilder.Entity<LifecycleProgram>()
            .HasMany(p => p.LifecycleStages)
            .WithOne()
            .HasForeignKey("LifecycleProgramId")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LifecycleProgram>()
            .Navigation(p => p.LifecycleStages)
            .AutoInclude();

        modelBuilder.Entity<LifecycleStage>()
            .Navigation(ls => ls.Ration)
            .AutoInclude();

        modelBuilder.Entity<LifecycleStage>()
            .Navigation(ls => ls.GrowthTreatment)
            .AutoInclude();

        modelBuilder.Entity<LifecycleStage>()
            .Navigation(ls => ls.PreventativeTreatment)
            .AutoInclude();
    }
}

