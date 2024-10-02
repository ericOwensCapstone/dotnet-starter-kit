using MediatR;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.Get.v1;
public class GetGrowthTreatmentRequest : IRequest<GrowthTreatmentResponse>
{
    public Guid Id { get; set; }
    public GetGrowthTreatmentRequest(Guid id) => Id = id;
}

