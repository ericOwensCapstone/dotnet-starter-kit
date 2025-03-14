
namespace FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Get.v1;

public sealed record PreventiveTreatmentResponse(
    Guid? Id,
    string Name,
    string? Description,
    decimal DollarsPerHead
);