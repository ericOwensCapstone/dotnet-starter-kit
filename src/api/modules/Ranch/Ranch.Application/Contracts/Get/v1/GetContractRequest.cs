using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Get.v1;
public class GetContractRequest : IRequest<ContractResponse>
{
    public Guid Id { get; set; }
    public GetContractRequest(Guid id) => Id = id;
}

