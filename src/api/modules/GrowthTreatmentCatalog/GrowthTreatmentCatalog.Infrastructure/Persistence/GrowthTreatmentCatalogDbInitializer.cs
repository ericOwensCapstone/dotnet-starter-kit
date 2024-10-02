using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Infrastructure.Persistence;
internal sealed class GrowthTreatmentCatalogDbInitializer(
    ILogger<GrowthTreatmentCatalogDbInitializer> logger,
    GrowthTreatmentCatalogDbContext context) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await context.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[{Tenant}] applied database migrations for growthTreatmentcatalog module", context.TenantInfo!.Identifier);
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        const string Name = "GrowthTreatment1";
        const string Description = "Growth Treatment 1";
        const decimal DollarsPerHead = 1;
        if (await context.GrowthTreatments.FirstOrDefaultAsync(t => t.Name == Name, cancellationToken).ConfigureAwait(false) is null)
        {
            var growthTreatment = GrowthTreatment.Create(Name, Description, DollarsPerHead);
            await context.GrowthTreatments.AddAsync(growthTreatment, cancellationToken);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[{Tenant}] seeding default growthTreatmentcatalog data", context.TenantInfo!.Identifier);
        }
    }
}

