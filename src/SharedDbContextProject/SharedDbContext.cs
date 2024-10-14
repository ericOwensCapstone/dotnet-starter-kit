using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            modelBuilder.Entity<LifecycleProgram>()
                .HasMany(lp => lp.LifecycleStages)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "LifecycleProgramLifecycleStage",
                    j => j.HasOne<LifecycleStage>().WithMany().HasForeignKey("LifecycleStageId"),
                    j => j.HasOne<LifecycleProgram>().WithMany().HasForeignKey("LifecycleProgramId"));

            modelBuilder.Entity<LifecycleProgram>()
                .Navigation(lp => lp.LifecycleStages)
                .AutoInclude();
        }
    }
}
