using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedDbContextProject;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Infrastructure.Persistence;
internal sealed class GrowthTreatmentCatalogDbInitializer(
    ILogger<GrowthTreatmentCatalogDbInitializer> logger,
    SharedDbContext context) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(5, cancellationToken);
        //if ((await context.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        //{
        //    await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        //    logger.LogInformation("[{Tenant}] applied database migrations for growthTreatmentcatalog module", context.TenantInfo!.Identifier);
        //}
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(5, cancellationToken);
    }
}

