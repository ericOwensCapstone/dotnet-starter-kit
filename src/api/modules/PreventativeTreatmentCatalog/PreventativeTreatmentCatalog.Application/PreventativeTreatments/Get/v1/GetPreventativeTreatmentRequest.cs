using MediatR;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Get.v1;
public class GetPreventativeTreatmentRequest : IRequest<PreventativeTreatmentResponse>
{
    public Guid Id { get; set; }
    public GetPreventativeTreatmentRequest(Guid id) => Id = id;
}

