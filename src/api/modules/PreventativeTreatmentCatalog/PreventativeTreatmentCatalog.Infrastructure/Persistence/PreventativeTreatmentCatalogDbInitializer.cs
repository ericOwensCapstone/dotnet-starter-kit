using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedDbContextProject;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Infrastructure.Persistence;
internal sealed class PreventativeTreatmentCatalogDbInitializer(
    ILogger<PreventativeTreatmentCatalogDbInitializer> logger,
    SharedDbContext context) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(5, cancellationToken);
        //if ((await context.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        //{
        //    await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        //    logger.LogInformation("[{Tenant}] applied database migrations for preventativeTreatmentcatalog module", context.TenantInfo!.Identifier);
        //}
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(5, cancellationToken);
        //const string Name = "PreventativeTreatment1";
        //const string Description = "PreventativeTreatment1";
        //const decimal DollarsPerHead = 1.55m;
        //if (await context.PreventativeTreatments.FirstOrDefaultAsync(t => t.Name == Name, cancellationToken).ConfigureAwait(false) is null)
        //{
        //    var preventativeTreatment = PreventativeTreatment.Create(Name, Description, DollarsPerHead);
        //    await context.PreventativeTreatments.AddAsync(preventativeTreatment, cancellationToken);
        //    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        //    logger.LogInformation("[{Tenant}] seeding default preventativeTreatmentcatalog data", context.TenantInfo!.Identifier);
        //}
    }
}

