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
using SharedDbContextProject;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure.Persistence;

public sealed class LifecycleStageCatalogDbContext : SharedDbContext
{
    public LifecycleStageCatalogDbContext(IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor, DbContextOptions<LifecycleStageCatalogDbContext> options, IPublisher publisher, IOptions<DatabaseOptions> settings)
        : base(multiTenantContextAccessor, options, publisher, settings)
    {
    }
    //public DbSet<Ration> Rations { get; set; } = null!;
    //public DbSet<GrowthTreatment> GrowthTreatments { get; set; } = null!;
    //public DbSet<PreventativeTreatment> PreventativeTreatments { get; set; } = null!;

    //public DbSet<LifecycleStage> LifecycleStages { get; set; } = null!;

    ////public DbSet<LifecycleProgramStage> LifecycleProgramStages { get; set; } = null!;

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    ArgumentNullException.ThrowIfNull(modelBuilder);
    //    modelBuilder.ApplyConfigurationsFromAssembly(typeof(LifecycleStageCatalogDbContext).Assembly);
    //    modelBuilder.HasDefaultSchema(SchemaNames.LifecycleStageCatalog);

    //    modelBuilder.Entity<Ration>()
    //        .ToTable("Rations", schema: "rationcatalog")
    //        .Metadata.SetIsTableExcludedFromMigrations(true);

    //    modelBuilder.Entity<GrowthTreatment>()
    //        .ToTable("GrowthTreatments", schema: "growthtreatmentcatalog")
    //        .Metadata.SetIsTableExcludedFromMigrations(true);

    //    modelBuilder.Entity<PreventativeTreatment>()
    //        .ToTable("PreventativeTreatments", schema: "preventativetreatmentcatalog")
    //        .Metadata.SetIsTableExcludedFromMigrations(true);

    //    //modelBuilder.Entity<LifecycleProgram>()
    //    //    .ToTable("LifecyclePrograms", schema: "lifecycleprogramcatalog")
    //    //    .Metadata.SetIsTableExcludedFromMigrations(true);

    //    //modelBuilder.Entity<LifecycleProgramStage>()
    //    //    .ToTable("LifecycleProgramStages", schema: "lifecycleprogramcatalog")
    //    //    .Metadata.SetIsTableExcludedFromMigrations(true);

    //    modelBuilder.Entity<LifecycleStage>()
    //        .ToTable("LifecycleStages", schema: "lifecyclestagecatalog");

    //    modelBuilder.Entity<LifecycleStage>()
    //        .Navigation(ls => ls.Ration)
    //        .AutoInclude();

    //    modelBuilder.Entity<LifecycleStage>()
    //        .Navigation(ls => ls.GrowthTreatment)
    //        .AutoInclude();

    //    modelBuilder.Entity<LifecycleStage>()
    //        .Navigation(ls => ls.PreventativeTreatment)
    //        .AutoInclude();

    //    //modelBuilder.Entity<LifecycleProgramStage>()
    //    //    .HasKey(lps => new { lps.LifecycleProgramId, lps.LifecycleStageId });

    //    //modelBuilder.Entity<LifecycleProgramStage>()
    //    //    .HasOne(lps => lps.LifecycleProgram)
    //    //    .WithMany(lp => lp.LifecycleProgramStages)
    //    //    .HasForeignKey(lps => lps.LifecycleProgramId);

    //    //modelBuilder.Entity<LifecycleProgramStage>()
    //    //    .HasOne(lps => lps.LifecycleStage)
    //    //    .WithMany(ls => ls.LifecycleProgramStages)
    //    //    .HasForeignKey(lps => lps.LifecycleStageId);
    //}
}

