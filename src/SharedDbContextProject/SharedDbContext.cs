using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.WebApi.AnimalTypeCatalog.Domain;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using FSH.Starter.WebApi.RationCatalog.Domain;
using FSH.Starter.WebApi.WeatherZoneCatalog.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace SharedDbContextProject
{
    public class SharedDbContext : FshDbContext
    {
        public SharedDbContext(IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor, DbContextOptions options, IPublisher publisher, IOptions<DatabaseOptions> settings)
            : base(multiTenantContextAccessor, options, publisher, settings)
        {
        }

        public DbSet<Ration> Rations { get; set; } = null!;
        public DbSet<GrowthTreatment> GrowthTreatments { get; set; } = null!;
        public DbSet<PreventativeTreatment> PreventativeTreatments { get; set; } = null!;
        public DbSet<LifecycleStage> LifecycleStages { get; set; } = null!;
        public DbSet<LifecycleProgram> LifecyclePrograms { get; set; } = null!;
        public DbSet<LifecycleProgramStage> LifecycleProgramStages { get; set; } = null!;
        public DbSet<AnimalType> AnimalTypes { get; set; } = null!;
        public DbSet<WeatherZone> WeatherZones { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SharedDbContext).Assembly);
            modelBuilder.HasDefaultSchema(SchemaNames.SharedCatalog);

            modelBuilder.Entity<LifecycleStage>()
                .Navigation(ls => ls.Ration)
                .AutoInclude();

            modelBuilder.Entity<LifecycleStage>()
                .Navigation(ls => ls.GrowthTreatment)
                .AutoInclude();

            modelBuilder.Entity<LifecycleStage>()
                .Navigation(ls => ls.PreventativeTreatment)
                .AutoInclude();

            modelBuilder.Entity<LifecycleProgramStage>()
                .HasKey(lps => new { lps.LifecycleProgramId, lps.LifecycleStageId });

            modelBuilder.Entity<LifecycleProgramStage>()
                .HasOne(lps => lps.LifecycleProgram)
                .WithMany(lp => lp.LifecycleProgramStages)
                .HasForeignKey(lps => lps.LifecycleProgramId);

            modelBuilder.Entity<LifecycleProgramStage>()
                .HasOne(lps => lps.LifecycleStage)
                .WithMany()
                .HasForeignKey(lps => lps.LifecycleStageId);

            modelBuilder.Entity<LifecycleProgram>()
                .Navigation(lp => lp.LifecycleProgramStages)
                .AutoInclude();

            modelBuilder.Entity<LifecycleProgramStage>()
                .Navigation(lps => lps.LifecycleStage)
                .AutoInclude();

        }
    }
}
