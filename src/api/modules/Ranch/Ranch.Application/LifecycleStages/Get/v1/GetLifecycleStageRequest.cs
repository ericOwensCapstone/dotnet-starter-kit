using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.LifecycleStages.Get.v1;
public class GetLifecycleStageRequest : IRequest<LifecycleStageResponse>
{
    public Guid Id { get; set; }
    public GetLifecycleStageRequest(Guid id) => Id = id;
}

