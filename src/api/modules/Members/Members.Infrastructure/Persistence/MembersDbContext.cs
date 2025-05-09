
using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shared.Constants;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.Tenant;

// Start MemberAd
using FSH.Starter.WebApi.Members.Domain.MemberAds;
// End MemberAd
namespace FSH.Starter.WebApi.Members.Infrastructure.Persistence;

public sealed class MembersDbContext : FshDbContext
{
    public MembersDbContext(IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor, DbContextOptions<MembersDbContext> options, IPublisher publisher, IOptions<DatabaseOptions> settings)
        : base(multiTenantContextAccessor, options, publisher, settings)
    {
    }

    // Start MemberAd
    public DbSet<MemberAd> MemberAds { get; set; } = null!;
    // End MemberAd
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MembersDbContext).Assembly);
        modelBuilder.HasDefaultSchema(SchemaNames.Members);
    }
}
