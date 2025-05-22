using MediatR;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Get.v1;
public class GetHarvestContractStatusRequest : IRequest<HarvestContractStatusResponse>
{
    public Guid Id { get; set; }
    public GetHarvestContractStatusRequest(Guid id) => Id = id;
}

