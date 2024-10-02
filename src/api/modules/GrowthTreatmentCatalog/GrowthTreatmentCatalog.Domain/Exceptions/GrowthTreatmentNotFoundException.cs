using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain.Exceptions;
public sealed class GrowthTreatmentNotFoundException : NotFoundException
{
    public GrowthTreatmentNotFoundException(Guid id)
        : base($"growthTreatment with id {id} not found")
    {
    }
}

