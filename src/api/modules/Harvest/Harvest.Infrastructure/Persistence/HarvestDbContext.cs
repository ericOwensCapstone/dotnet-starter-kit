
using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Constants;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.Tenant;

// Start HarvestContractStatus;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses;
// End HarvestContractStatus;
// Start HarvestMember;
using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers;
// End HarvestMember;
// Start HarvestContract;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts;
// End HarvestContract;
namespace FSH.Starter.WebApi.Harvest.Infrastructure.Persistence;

public sealed class HarvestDbContext : FshDbContext
{
    public HarvestDbContext(IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor, DbContextOptions<HarvestDbContext> options, IPublisher publisher, IOptions<DatabaseOptions> settings)
        : base(multiTenantContextAccessor, options, publisher, settings)
    {
    }

    // Start HarvestContractStatus;
    public DbSet<HarvestContractStatus> HarvestContractStatuses { get; set; } = null!;
    // End HarvestContractStatus;
    // Start HarvestMember;
    public DbSet<HarvestMember> HarvestMembers { get; set; } = null!;
    // End HarvestMember;
    // Start HarvestContract;
    public DbSet<HarvestContract> HarvestContracts { get; set; } = null!;
    // End HarvestContract;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(HarvestDbContext).Assembly);
        modelBuilder.HasDefaultSchema(SchemaNames.Harvest);
    }
}
