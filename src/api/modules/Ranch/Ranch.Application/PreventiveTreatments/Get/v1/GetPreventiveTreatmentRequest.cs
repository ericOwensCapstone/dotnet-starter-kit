using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Get.v1;
public class GetPreventiveTreatmentRequest : IRequest<PreventiveTreatmentResponse>
{
    public Guid Id { get; set; }
    public GetPreventiveTreatmentRequest(Guid id) => Id = id;
}

