using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Starter.WebApi.AnimalTypeCatalog.Domain;
using FSH.Starter.WebApi.AnimalTypeCatalog.Infrastructure.Endpoints.v1;
using FSH.Starter.WebApi.AnimalTypeCatalog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SharedDbContextProject;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Infrastructure;
public static class AnimalTypeCatalogModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("animalTypecatalog") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var animalTypeGroup = app.MapGroup("animalTypes").WithTags("animalTypes");
            animalTypeGroup.MapAnimalTypeCreationEndpoint();
            animalTypeGroup.MapGetAnimalTypeEndpoint();
            animalTypeGroup.MapGetAnimalTypeListEndpoint();
            animalTypeGroup.MapAnimalTypeUpdateEndpoint();
            animalTypeGroup.MapAnimalTypeDeleteEndpoint();
        }
    }
    public static WebApplicationBuilder RegisterAnimalTypeCatalogServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<SharedDbContext>();
        builder.Services.AddKeyedScoped<IRepository<AnimalType>, AnimalTypeCatalogRepository<AnimalType>>("animalTypecatalog:animalTypes");
        builder.Services.AddKeyedScoped<IReadRepository<AnimalType>, AnimalTypeCatalogRepository<AnimalType>>("animalTypecatalog:animalTypes");
        return builder;
    }
    public static WebApplication UseAnimalTypeCatalogModule(this WebApplication app)
    {
        return app;
    }
}


