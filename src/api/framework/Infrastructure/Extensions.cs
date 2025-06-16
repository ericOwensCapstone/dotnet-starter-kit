using System.Reflection;
using Asp.Versioning.Conventions;
using Carter;
using FluentValidation;
using FSH.Framework.Core;
using FSH.Framework.Core.Origin;
using FSH.Framework.Infrastructure.Auth;
using FSH.Framework.Infrastructure.Auth.ApiKey.Endpoints;
using FSH.Framework.Infrastructure.Auth.AzureB2C.Endpoints;
using FSH.Framework.Infrastructure.Auth.Jwt;
using FSH.Framework.Infrastructure.Behaviours;
using FSH.Framework.Infrastructure.Caching;
using FSH.Framework.Infrastructure.Cors;
using FSH.Framework.Infrastructure.Exceptions;
using FSH.Framework.Infrastructure.Graph;
using FSH.Framework.Infrastructure.Identity;
using FSH.Framework.Infrastructure.Jobs;
using FSH.Framework.Infrastructure.Logging.Serilog;
using FSH.Framework.Infrastructure.Mail;
using FSH.Framework.Infrastructure.OpenApi;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.RateLimit;
using FSH.Framework.Infrastructure.SecurityHeaders;
using FSH.Framework.Infrastructure.Storage.Files;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Framework.Infrastructure.Tenant.Endpoints;
using FSH.Starter.Aspire.ServiceDefaults;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace FSH.Framework.Infrastructure;

public static class Extensions
{
    public static WebApplicationBuilder ConfigureFshFramework(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.AddServiceDefaults();
        builder.ConfigureSerilog();
        builder.ConfigureDatabase();
        builder.Services.ConfigureMultitenancy();
        builder.Services.ConfigureIdentity();
        builder.Services.AddCorsPolicy(builder.Configuration);
        builder.Services.ConfigureFileStorage();
        builder.Services.ConfigureAuthentication(builder.Configuration);
        builder.Services.AddGraphServices(builder.Configuration);
        builder.Services.ConfigureOpenApi();
        builder.Services.ConfigureJobs(builder.Configuration);
        builder.Services.ConfigureMailing();
        builder.Services.ConfigureCaching(builder.Configuration);
        builder.Services.AddExceptionHandler<CustomExceptionHandler>();
        builder.Services.AddProblemDetails();
        builder.Services.AddHealthChecks();
        builder.Services.AddOptions<OriginOptions>().BindConfiguration(nameof(OriginOptions));

        // Define module assemblies
        var assemblies = new Assembly[]
        {
            typeof(FshCore).Assembly,
            typeof(FshInfrastructure).Assembly
        };

        // Register validators
        builder.Services.AddValidatorsFromAssemblies(assemblies);

        // Register MediatR
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        builder.Services.ConfigureRateLimit(builder.Configuration);
        builder.Services.ConfigureSecurityHeaders(builder.Configuration);

        return builder;
    }

    public static WebApplication UseFshFramework(this WebApplication app)
    {
        var env = app.Services.GetRequiredService<IWebHostEnvironment>();

        ////Conditionally add the DevelopmentCorsMiddleware in development environment
        //if (env.IsDevelopment())
        //{
        //    app.UseMiddleware<DevelopmentCorsMiddleware>();
        //}

        app.MapDefaultEndpoints();
        app.UseRateLimit();
        app.UseSecurityHeaders();
        app.UseMultitenancy();
        app.UseExceptionHandler();
        app.UseCorsPolicy();
        app.UseOpenApi();
        app.UseJobDashboard(app.Configuration);
        app.UseRouting();
        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions()
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "assets")),
            RequestPath = new PathString("/assets")
        });
        app.UseAuthentication();
        
        // Current user middleware - MUST be after authentication but before authorization
        app.UseMiddleware<CurrentUserMiddleware>();
        
        app.UseAuthorization();
        
        // Add middleware to log all requests to /api/public
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/api/public"))
            {
                var reqLogger = context.RequestServices.GetRequiredService<ILogger<WebApplication>>();
                reqLogger.LogInformation("=== PUBLIC API REQUEST ===");
                reqLogger.LogInformation("Path: {Path}", context.Request.Path);
                reqLogger.LogInformation("Method: {Method}", context.Request.Method);
                reqLogger.LogInformation("QueryString: {QueryString}", context.Request.QueryString);
                reqLogger.LogInformation("Headers: {@Headers}", context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));
            }
            await next();
        });
        
        // Add logging for endpoint mapping
        var logger = app.Services.GetRequiredService<ILogger<WebApplication>>();
        logger.LogInformation("=== STARTING ENDPOINT MAPPING ===");
        
        logger.LogInformation("Mapping Tenant Endpoints...");
        app.MapTenantEndpoints();
        
        logger.LogInformation("Mapping Identity Endpoints...");
        app.MapIdentityEndpoints();
        
        logger.LogInformation("Mapping ApiKey Endpoints...");
        app.MapApiKeyEndpoints();
        
        logger.LogInformation("About to map B2C Endpoints...");
        app.MapB2CEndpoints();
        logger.LogInformation("B2C Endpoints mapped successfully");
        
        logger.LogInformation("=== ENDPOINT MAPPING COMPLETE ===");

        return app;
    }
}
