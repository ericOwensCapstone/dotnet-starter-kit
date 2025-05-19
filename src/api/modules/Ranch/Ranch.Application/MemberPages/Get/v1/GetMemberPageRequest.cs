using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.MemberPages.Get.v1;
public class GetMemberPageRequest : IRequest<MemberPageResponse>
{
    public Guid Id { get; set; }
    public GetMemberPageRequest(Guid id) => Id = id;
}

