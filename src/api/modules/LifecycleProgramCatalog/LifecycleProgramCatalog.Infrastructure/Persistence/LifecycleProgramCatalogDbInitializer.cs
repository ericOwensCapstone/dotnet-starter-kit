using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Infrastructure.Persistence;
internal sealed class LifecycleProgramCatalogDbInitializer(
    ILogger<LifecycleProgramCatalogDbInitializer> logger,
    LifecycleProgramCatalogDbContext context) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await context.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[{Tenant}] applied database migrations for lifecycleProgramcatalog module", context.TenantInfo!.Identifier);
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(10);
    }
}

