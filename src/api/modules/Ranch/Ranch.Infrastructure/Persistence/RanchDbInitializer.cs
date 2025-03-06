using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Persistence;
internal sealed class RanchDbInitializer(
    ILogger<RanchDbInitializer> logger,
    RanchDbContext context) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await context.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
        {
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[{Tenant}] applied database migrations for ranch module", context.TenantInfo!.Identifier);
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        const string Name = "Keychron V6 QMK Custom Wired Mechanical Keyboard";
        const string Description = "A full-size layout QMK/VIA custom mechanical keyboard";
        const decimal Price = 79;
        if (await context.Rations.FirstOrDefaultAsync(t => t.Name == Name, cancellationToken).ConfigureAwait(false) is null)
        {
            var ration = Ration.Create(Name, Description, Price);
            await context.Rations.AddAsync(ration, cancellationToken);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("[{Tenant}] seeding default ranch data", context.TenantInfo!.Identifier);
        }
    }
}

