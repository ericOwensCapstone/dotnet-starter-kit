using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Identity.Persistence;
using FSH.Framework.Infrastructure.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;

namespace FSH.Starter.WebApi.Migrations.PostgreSQL;

public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var connectionString = "Server=localhost;Database=fshdb;User Id=postgres;Password=postgres;";
        
        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        // Create a mock multi-tenant context
        var tenantInfo = new FshTenantInfo
        {
            Id = "root",
            Identifier = "root",
            Name = "Root Tenant",
            ConnectionString = connectionString
        };

        var mockMultiTenantContextAccessor = new MockMultiTenantContextAccessor(tenantInfo);
        var databaseOptions = Options.Create(new DatabaseOptions { Provider = "postgresql" });

        return new IdentityDbContext(mockMultiTenantContextAccessor, optionsBuilder.Options, databaseOptions);
    }
}

public class MockMultiTenantContextAccessor : IMultiTenantContextAccessor<FshTenantInfo>
{
    public MockMultiTenantContextAccessor(FshTenantInfo tenantInfo)
    {
        MultiTenantContext = new MockMultiTenantContext(tenantInfo);
    }

    public IMultiTenantContext<FshTenantInfo> MultiTenantContext { get; }
    
    IMultiTenantContext IMultiTenantContextAccessor.MultiTenantContext => MultiTenantContext;
}

public class MockMultiTenantContext : IMultiTenantContext<FshTenantInfo>
{
    public MockMultiTenantContext(FshTenantInfo tenantInfo)
    {
        TenantInfo = tenantInfo;
        StrategyInfo = null;
        StoreInfo = null;
    }

    public FshTenantInfo? TenantInfo { get; set; }
    public StrategyInfo? StrategyInfo { get; set; }
    public StoreInfo<FshTenantInfo>? StoreInfo { get; set; }
    public bool IsResolved => true;

    ITenantInfo? IMultiTenantContext.TenantInfo => TenantInfo;

    public bool TryDispose()
    {
        return true;
    }
}