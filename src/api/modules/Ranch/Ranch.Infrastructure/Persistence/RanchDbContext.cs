using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.WebApi.Ranch.Domain.Rations;
using FSH.Starter.WebApi.Ranch.Domain.GrowthTreatments;
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments;
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
    public DbSet<GrowthTreatment> GrowthTreatments { get; set; } = null!;
    public DbSet<PreventiveTreatment> PreventiveTreatments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RanchDbContext).Assembly);
        modelBuilder.HasDefaultSchema(SchemaNames.Ranch);
    }
}

