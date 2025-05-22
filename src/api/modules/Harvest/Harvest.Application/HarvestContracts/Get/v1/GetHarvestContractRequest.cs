using MediatR;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Get.v1;
public class GetHarvestContractRequest : IRequest<HarvestContractResponse>
{
    public Guid Id { get; set; }
    public GetHarvestContractRequest(Guid id) => Id = id;
}

