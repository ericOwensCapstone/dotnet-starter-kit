using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.WebApi.Ranch.Domain.Rations;
using FSH.Starter.WebApi.Ranch.Domain.GrowthTreatments;
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;
using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Constants;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Persistence;

public sealed class RanchDbContext : FshDbContext
{
    public RanchDbContext(IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor, DbContextOptions<RanchDbContext> options, IPublisher publisher, IOptions<DatabaseOptions> settings)
        : base(multiTenantContextAccessor, options, publisher, settings)
    {
    }

    public DbSet<Ration> Rations { get; set; } = null!;
    // Start GrowthTreatment
    public DbSet<GrowthTreatment> GrowthTreatments { get; set; } = null!;
    // End GrowthTreatment
    // Start PreventiveTreatment
    public DbSet<PreventiveTreatment> PreventiveTreatments { get; set; } = null!;
    // End PreventiveTreatment
    // Start LifecycleStage
    public DbSet<LifecycleStage> LifecycleStages { get; set; } = null!;
    // End LifecycleStage
    // Start LifecycleProgram
    public DbSet<LifecycleProgram> LifecyclePrograms { get; set; } = null!;
    // End LifecycleProgram

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RanchDbContext).Assembly);
        modelBuilder.HasDefaultSchema(SchemaNames.Ranch);
    }
}

