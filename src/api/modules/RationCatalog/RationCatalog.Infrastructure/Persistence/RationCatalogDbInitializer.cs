using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.RationCatalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedDbContextProject;

namespace FSH.Starter.WebApi.RationCatalog.Infrastructure.Persistence;
internal sealed class RationCatalogDbInitializer(
    ILogger<RationCatalogDbInitializer> logger,
    SharedDbContext context) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(5, cancellationToken);
        //if ((await context.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        //{
        //    await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        //    logger.LogInformation("[{Tenant}] applied database migrations for rationcatalog module", context.TenantInfo!.Identifier);
        //}
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(5, cancellationToken);
        //const string Name = "Ration1";
        //const string Description = "Ration1";
        //const decimal Price = 0.10m;
        //if (await context.Rations.FirstOrDefaultAsync(t => t.Name == Name, cancellationToken).ConfigureAwait(false) is null)
        //{
        //    var ration = Ration.Create(Name, Description, Price);
        //    await context.Rations.AddAsync(ration, cancellationToken);
        //    await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        //    logger.LogInformation("[{Tenant}] seeding default rationcatalog data", context.TenantInfo!.Identifier);
        //}
    }
}

