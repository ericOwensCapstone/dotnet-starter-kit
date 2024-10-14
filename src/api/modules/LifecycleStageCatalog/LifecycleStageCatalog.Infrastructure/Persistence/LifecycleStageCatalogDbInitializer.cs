using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using FSH.Starter.WebApi.RationCatalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedDbContextProject;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure.Persistence;
internal sealed class LifecycleStageCatalogDbInitializer(
    ILogger<LifecycleStageCatalogDbInitializer> logger,
    SharedDbContext context) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(5, cancellationToken);
        //if ((await context.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        //{
        //    await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        //    logger.LogInformation("[{Tenant}] applied database migrations for lifecycleStagecatalog module", context.TenantInfo!.Identifier);
        //}
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(5, cancellationToken);
    }
}

