using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Ranch.Infrastructure.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.Rations;
using FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.Rations;
// Start GrowthTreatment
using FSH.Starter.WebApi.Ranch.Domain.GrowthTreatments;
using FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.GrowthTreatments;
// End GrowthTreatment
// Start PreventiveTreatment
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments;
using FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.PreventiveTreatments;
// End PreventiveTreatment
// Start LifecycleStage
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;
using FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.LifecycleStages;
// End LifecycleStage
namespace FSH.Starter.WebApi.Ranch.Infrastructure;
public static class RanchModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("ranch") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var rationGroup = app.MapGroup("rations").WithTags("rations");
            rationGroup.MapRationCreationEndpoint();
            rationGroup.MapGetRationEndpoint();
            rationGroup.MapGetRationListEndpoint();
            rationGroup.MapRationUpdateEndpoint();
            rationGroup.MapRationDeleteEndpoint();
        
            // Start GrowthTreatment
            var growthTreatmentGroup = app.MapGroup("growthTreatments").WithTags("growthTreatments");
            growthTreatmentGroup.MapGrowthTreatmentCreationEndpoint();
            growthTreatmentGroup.MapGetGrowthTreatmentEndpoint();
            growthTreatmentGroup.MapGetGrowthTreatmentListEndpoint();
            growthTreatmentGroup.MapGrowthTreatmentUpdateEndpoint();
            growthTreatmentGroup.MapGrowthTreatmentDeleteEndpoint();
            // End GrowthTreatment

            // Start PreventiveTreatment
            var preventiveTreatmentGroup = app.MapGroup("preventiveTreatments").WithTags("preventiveTreatments");
            preventiveTreatmentGroup.MapPreventiveTreatmentCreationEndpoint();
            preventiveTreatmentGroup.MapGetPreventiveTreatmentEndpoint();
            preventiveTreatmentGroup.MapGetPreventiveTreatmentListEndpoint();
            preventiveTreatmentGroup.MapPreventiveTreatmentUpdateEndpoint();
            preventiveTreatmentGroup.MapPreventiveTreatmentDeleteEndpoint();
            // End PreventiveTreatment

            // Start LifecycleStage
            var lifecycleStageGroup = app.MapGroup("lifecycleStages").WithTags("lifecycleStages");
            lifecycleStageGroup.MapLifecycleStageCreationEndpoint();
            lifecycleStageGroup.MapGetLifecycleStageEndpoint();
            lifecycleStageGroup.MapGetLifecycleStageListEndpoint();
            lifecycleStageGroup.MapLifecycleStageUpdateEndpoint();
            lifecycleStageGroup.MapLifecycleStageDeleteEndpoint();
            // End LifecycleStage

            
        }
    }
    public static WebApplicationBuilder RegisterRanchServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<RanchDbContext>();
        builder.Services.AddScoped<IDbInitializer, RanchDbInitializer>();
        builder.Services.AddKeyedScoped<IRepository<Ration>, RanchRepository<Ration>>("ranch:rations");
        builder.Services.AddKeyedScoped<IReadRepository<Ration>, RanchRepository<Ration>>("ranch:rations");
        // Start GrowthTreatment
        builder.Services.AddKeyedScoped<IRepository<GrowthTreatment>, RanchRepository<GrowthTreatment>>("ranch:growthTreatments");
        builder.Services.AddKeyedScoped<IReadRepository<GrowthTreatment>, RanchRepository<GrowthTreatment>>("ranch:growthTreatments");
        // End GrowthTreatment
        // Start PreventiveTreatment
        builder.Services.AddKeyedScoped<IRepository<PreventiveTreatment>, RanchRepository<PreventiveTreatment>>("ranch:preventiveTreatments");
        builder.Services.AddKeyedScoped<IReadRepository<PreventiveTreatment>, RanchRepository<PreventiveTreatment>>("ranch:preventiveTreatments");
        // End PreventiveTreatment
        // Start LifecycleStage
        builder.Services.AddKeyedScoped<IRepository<LifecycleStage>, RanchRepository<LifecycleStage>>("ranch:lifecycleStages");
        builder.Services.AddKeyedScoped<IReadRepository<LifecycleStage>, RanchRepository<LifecycleStage>>("ranch:lifecycleStages");
        // End LifecycleStage
        
        return builder;
    }
    public static WebApplication UseRanchModule(this WebApplication app)
    {
        return app;
    }
}

