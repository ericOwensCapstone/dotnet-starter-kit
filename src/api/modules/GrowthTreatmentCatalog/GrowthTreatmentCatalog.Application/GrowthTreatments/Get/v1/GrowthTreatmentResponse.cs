namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.Get.v1;
public sealed record GrowthTreatmentResponse(Guid? Id, string Name, string? Description, decimal DollarsPerHead);

