using Ardalis.Specification;
using FSH.Starter.WebApi.Ranch.Domain.Rations;

namespace FSH.Starter.WebApi.Ranch.Application.Rations.Get.v1;

public class GetRationSpecs : Specification<Ration, RationResponse>
{
    public GetRationSpecs(Guid id)
    {
        Query
            .Where(p => p.Id == id);
    }
}

