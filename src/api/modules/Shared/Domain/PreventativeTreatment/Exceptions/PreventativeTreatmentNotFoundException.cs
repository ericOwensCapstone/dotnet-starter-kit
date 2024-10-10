using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain.Exceptions;
public sealed class PreventativeTreatmentNotFoundException : NotFoundException
{
    public PreventativeTreatmentNotFoundException(Guid id)
        : base($"preventativeTreatment with id {id} not found")
    {
    }
}

