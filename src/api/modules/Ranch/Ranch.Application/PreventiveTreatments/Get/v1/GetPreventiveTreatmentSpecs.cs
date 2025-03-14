using Ardalis.Specification;
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments;

namespace FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Get.v1;

public class GetPreventiveTreatmentSpecs : Specification<PreventiveTreatment, PreventiveTreatmentResponse>
{
    public GetPreventiveTreatmentSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id);
    }
}

