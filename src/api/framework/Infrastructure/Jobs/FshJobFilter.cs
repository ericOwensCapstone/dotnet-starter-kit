using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Infrastructure.Constants;
using FSH.Starter.Shared.Authorization;
using Hangfire.Client;
using Hangfire.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Framework.Infrastructure.Jobs;

public class FshJobFilter : IClientFilter
{
    private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

    private readonly IServiceProvider _services;

    public FshJobFilter(IServiceProvider services) => _services = services;

    public void OnCreating(CreatingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        Logger.InfoFormat("Set TenantId and UserId parameters to job {0}.{1}...", context.Job.Method.ReflectedType?.FullName, context.Job.Method.Name);

        using var scope = _services.CreateScope();

        var httpContext = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>()?.HttpContext;
        
        // During application startup or background processing, there might not be an HTTP context
        if (httpContext == null)
        {
            Logger.InfoFormat("No HttpContext available for job {0}.{1}, using system context", 
                context.Job.Method.ReflectedType?.FullName, context.Job.Method.Name);
            
            // Set default/system context parameters
            context.SetJobParameter(TenantConstants.Identifier, "root"); // Use root tenant for system jobs
            context.SetJobParameter(QueryStringKeys.UserId, "system"); // Use system user for background jobs
            return;
        }

        var tenantInfo = scope.ServiceProvider.GetRequiredService<IMultiTenantContextAccessor>().MultiTenantContext?.TenantInfo;
        context.SetJobParameter(TenantConstants.Identifier, tenantInfo?.Identifier ?? "root");

        string? userId = httpContext.User.GetUserId();
        context.SetJobParameter(QueryStringKeys.UserId, userId ?? "system");
    }

    public void OnCreated(CreatedContext context) =>
        Logger.InfoFormat(
            "Job created with parameters {0}",
            context.Parameters.Select(x => x.Key + "=" + x.Value).Aggregate((s1, s2) => s1 + ";" + s2));
}
