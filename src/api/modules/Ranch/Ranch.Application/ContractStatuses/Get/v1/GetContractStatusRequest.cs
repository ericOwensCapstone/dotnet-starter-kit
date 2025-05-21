using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Get.v1;
public class GetContractStatusRequest : IRequest<ContractStatusResponse>
{
    public Guid Id { get; set; }
    public GetContractStatusRequest(Guid id) => Id = id;
}

