
namespace FSH.Starter.WebApi.Ranch.Application.Rations.Get.v1;

public sealed record RationResponse(
    Guid? Id,
    string? TenantId,
    string Name,
    string? Description,
    decimal DollarsPerPound
);
