using MediatR;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Get.v1;
public class GetHarvestMemberRequest : IRequest<HarvestMemberResponse>
{
    public Guid Id { get; set; }
    public GetHarvestMemberRequest(Guid id) => Id = id;
}

