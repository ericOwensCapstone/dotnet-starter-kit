using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.Rations;
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

    }
}

