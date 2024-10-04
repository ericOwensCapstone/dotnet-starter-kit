using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using FSH.Starter.WebApi.RationCatalog.Domain;
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
        const string Name = "LifecycleStageTemp";
        const string Description = "LifecycleStageTemp";
        const decimal Rating = 5;

        Ration Ration = Ration.Create("RationTemp", "RationTemp", 0.01m);
        GrowthTreatment GrowthTreatment = GrowthTreatment.Create("GrowthTreatmentTemp", "GrowthTreatmentTemp", 0.01m);
        PreventativeTreatment PreventativeTreatment = PreventativeTreatment.Create("PreventativeTreatmentTemp", "PreventativeTreatmentTemp", 0.01m);
        try
        {
            if (await context.LifecycleStages.FirstOrDefaultAsync(t => t.Name == Name, cancellationToken).ConfigureAwait(false) is null)
            {
                var lifecycleStage = LifecycleStage.Create(Name, Description, Rating, Ration, GrowthTreatment, PreventativeTreatment);
                var entities = new List<object> { Ration, GrowthTreatment, PreventativeTreatment };
                await context.AddRangeAsync(entities, cancellationToken);
                await context.LifecycleStages.AddAsync(lifecycleStage, cancellationToken);
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                logger.LogInformation("[{Tenant}] seeding default lifecycleStagecatalog data", context.TenantInfo!.Identifier);
            }
        }
        catch (Exception ex)
        {
            var wd = 40;
        }
    }
}

