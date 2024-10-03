namespace FSH.Starter.WebApi.RationCatalog.Application.Rations.Get.v1;
public sealed record RationResponse(Guid? Id, string Name, string? Description, decimal DollarsPerPound);

