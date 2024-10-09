using MediatR;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Get.v1;
public class GetLifecycleProgramRequest : IRequest<LifecycleProgramResponse>
{
    public Guid Id { get; set; }
    public GetLifecycleProgramRequest(Guid id) => Id = id;
}

