using Carter;
using FSH.WebApi.Modules.AnimalTypeCatalog.Features.AnimalTypes.AnimalTypeCreation.v1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.WebApi.Modules.AnimalTypeCatalog;

public static class AnimalTypeCatalogModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("animalTypecatalog") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var animalTypeGroup = app.MapGroup("animalTypes").WithTags("animalTypes");
            animalTypeGroup.MapAnimalTypeCreationEndpoint();

            var testGroup = app.MapGroup("test").WithTags("test");
            testGroup.MapGet("/test", () => "hi");
        }
    }
    public static WebApplicationBuilder RegisterAnimalTypeCatalogServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder;
    }
    public static WebApplication UseAnimalTypeCatalogModule(this WebApplication app)
    {
        return app;
    }
}


