using FSH.Starter.WebApi.Members.Domain.MemberAds;

namespace FSH.Starter.WebApi.Members.Application.MemberAds.Get.v1;

public sealed record MemberAdResponse(
    Guid? Id,
    string? TenantId,
    string Name,
    string? Description
);