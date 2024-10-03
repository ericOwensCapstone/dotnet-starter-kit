namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Get.v1;
public sealed record PreventativeTreatmentResponse(Guid? Id, string Name, string? Description, decimal DollarsPerHead);

