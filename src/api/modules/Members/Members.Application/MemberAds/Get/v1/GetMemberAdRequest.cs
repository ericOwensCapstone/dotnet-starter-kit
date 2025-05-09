using MediatR;

namespace FSH.Starter.WebApi.Members.Application.MemberAds.Get.v1;
public class GetMemberAdRequest : IRequest<MemberAdResponse>
{
    public Guid Id { get; set; }
    public GetMemberAdRequest(Guid id) => Id = id;
}

