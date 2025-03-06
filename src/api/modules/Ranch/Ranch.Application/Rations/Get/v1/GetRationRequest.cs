using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.Rations.Get.v1;
public class GetRationRequest : IRequest<RationResponse>
{
    public Guid Id { get; set; }
    public GetRationRequest(Guid id) => Id = id;
}

