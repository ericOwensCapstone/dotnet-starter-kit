using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.Rations;
using FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.Rations;
using FSH.Starter.WebApi.Ranch.Infrastructure.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.GrowthTreatments;
using FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.GrowthTreatments;
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments;
using FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.PreventiveTreatments;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;
using FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.LifecycleStages;
using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms;
using FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.LifecyclePrograms;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

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
        
            var growthTreatmentGroup = app.MapGroup("growthTreatments").WithTags("growthTreatments");
            growthTreatmentGroup.MapGrowthTreatmentCreationEndpoint();
            growthTreatmentGroup.MapGetGrowthTreatmentEndpoint();
            growthTreatmentGroup.MapGetGrowthTreatmentListEndpoint();
            growthTreatmentGroup.MapGrowthTreatmentUpdateEndpoint();
            growthTreatmentGroup.MapGrowthTreatmentDeleteEndpoint();
        
            var preventiveTreatmentGroup = app.MapGroup("preventiveTreatments").WithTags("preventiveTreatments");
            preventiveTreatmentGroup.MapPreventiveTreatmentCreationEndpoint();
            preventiveTreatmentGroup.MapGetPreventiveTreatmentEndpoint();
            preventiveTreatmentGroup.MapGetPreventiveTreatmentListEndpoint();
            preventiveTreatmentGroup.MapPreventiveTreatmentUpdateEndpoint();
            preventiveTreatmentGroup.MapPreventiveTreatmentDeleteEndpoint();
        
            var lifecycleStageGroup = app.MapGroup("lifecycleStages").WithTags("lifecycleStages");
            lifecycleStageGroup.MapLifecycleStageCreationEndpoint();
            lifecycleStageGroup.MapGetLifecycleStageEndpoint();
            lifecycleStageGroup.MapGetLifecycleStageListEndpoint();
            lifecycleStageGroup.MapLifecycleStageUpdateEndpoint();
            lifecycleStageGroup.MapLifecycleStageDeleteEndpoint();
        
            var lifecycleProgramGroup = app.MapGroup("lifecyclePrograms").WithTags("lifecyclePrograms");
            lifecycleProgramGroup.MapLifecycleProgramCreationEndpoint();
            lifecycleProgramGroup.MapGetLifecycleProgramEndpoint();
            lifecycleProgramGroup.MapGetLifecycleProgramListEndpoint();
            lifecycleProgramGroup.MapLifecycleProgramUpdateEndpoint();
            lifecycleProgramGroup.MapLifecycleProgramDeleteEndpoint();
        }
    }
    public static WebApplicationBuilder RegisterRanchServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<RanchDbContext>();
        builder.Services.AddScoped<IDbInitializer, RanchDbInitializer>();
        builder.Services.AddKeyedScoped<IRepository<Ration>, RanchRepository<Ration>>("ranch:rations");
        builder.Services.AddKeyedScoped<IReadRepository<Ration>, RanchRepository<Ration>>("ranch:rations");
        builder.Services.AddKeyedScoped<IRepository<GrowthTreatment>, RanchRepository<GrowthTreatment>>("ranch:growthTreatments");
        builder.Services.AddKeyedScoped<IReadRepository<GrowthTreatment>, RanchRepository<GrowthTreatment>>("ranch:growthTreatments");
        builder.Services.AddKeyedScoped<IRepository<PreventiveTreatment>, RanchRepository<PreventiveTreatment>>("ranch:preventiveTreatments");
        builder.Services.AddKeyedScoped<IReadRepository<PreventiveTreatment>, RanchRepository<PreventiveTreatment>>("ranch:preventiveTreatments");
        builder.Services.AddKeyedScoped<IRepository<LifecycleStage>, RanchRepository<LifecycleStage>>("ranch:lifecycleStages");
        builder.Services.AddKeyedScoped<IReadRepository<LifecycleStage>, RanchRepository<LifecycleStage>>("ranch:lifecycleStages");
        builder.Services.AddKeyedScoped<IRepository<LifecycleProgram>, RanchRepository<LifecycleProgram>>("ranch:lifecyclePrograms");
        builder.Services.AddKeyedScoped<IReadRepository<LifecycleProgram>, RanchRepository<LifecycleProgram>>("ranch:lifecyclePrograms");
        return builder;
    }
    public static WebApplication UseRanchModule(this WebApplication app)
    {
        return app;
    }
}

