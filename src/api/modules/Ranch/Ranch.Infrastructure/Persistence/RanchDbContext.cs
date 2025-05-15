using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Constants;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.WebApi.Ranch.Domain.Rations;
// Start ContractStatus;
using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses;
// End ContractStatus;
// Start Contract;
using FSH.Starter.WebApi.Ranch.Domain.Contracts;
// End Contract;
namespace FSH.Starter.WebApi.Ranch.Infrastructure.Persistence;
public sealed class RanchDbContext : FshDbContext
{
    public RanchDbContext(IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor, DbContextOptions<RanchDbContext> options, IPublisher publisher, IOptions<DatabaseOptions> settings)
        : base(multiTenantContextAccessor, options, publisher, settings)
    {
    }
    public DbSet<Ration> Rations { get; set; } = null!;
    // Start ContractStatus;
    public DbSet<ContractStatus> ContractStatuses { get; set; } = null!;
    // End ContractStatus;
    // Start Contract;
    public DbSet<Contract> Contracts { get; set; } = null!;
    // End Contract;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RanchDbContext).Assembly);
        modelBuilder.HasDefaultSchema(SchemaNames.Ranch);
    }
}

