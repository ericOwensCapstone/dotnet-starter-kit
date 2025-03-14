using Ardalis.Specification;
using FSH.Starter.WebApi.Ranch.Domain.GrowthTreatments;

namespace FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Get.v1;

public class GetGrowthTreatmentSpecs : Specification<GrowthTreatment, GrowthTreatmentResponse>
{
    public GetGrowthTreatmentSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id);
    }
}

