using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure.Persistence;
internal sealed class LifecycleStageCatalogDbInitializer(
    ILogger<LifecycleStageCatalogDbInitializer> logger,
    LifecycleStageCatalogDbContext context) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await context.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[{Tenant}] applied database migrations for lifecycleStagecatalog module", context.TenantInfo!.Identifier);
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        const string Name = "LifecycleStage1";
        const string Description = "LifecycleStage1";
        const decimal Rating = 5;
        if (await context.LifecycleStages.FirstOrDefaultAsync(t => t.Name == Name, cancellationToken).ConfigureAwait(false) is null)
        {
            var lifecycleStage = LifecycleStage.Create(Name, Description, Rating);
            await context.LifecycleStages.AddAsync(lifecycleStage, cancellationToken);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[{Tenant}] seeding default lifecycleStagecatalog data", context.TenantInfo!.Identifier);
        }
    }
}

