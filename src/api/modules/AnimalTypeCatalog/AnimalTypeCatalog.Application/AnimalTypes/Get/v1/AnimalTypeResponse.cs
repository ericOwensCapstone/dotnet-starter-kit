namespace FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Get.v1;
public sealed record AnimalTypeResponse(Guid? Id, string Name, string? Description, decimal DollarsPerPound);


