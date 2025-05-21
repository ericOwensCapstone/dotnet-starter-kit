using FSH.Starter.WebApi.Ranch.Domain.MemberPages;

namespace FSH.Starter.WebApi.Ranch.Application.MemberPages.Get.v1;

public sealed record MemberPageResponse(
    Guid? Id,
    string? TenantId,
    Guid? MemberId,
    string Name,
    string? Description
);