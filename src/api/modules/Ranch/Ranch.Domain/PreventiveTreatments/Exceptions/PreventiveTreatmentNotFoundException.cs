using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments.Exceptions;
public sealed class PreventiveTreatmentNotFoundException : NotFoundException
{
    public PreventiveTreatmentNotFoundException(Guid id)
        : base($"preventiveTreatment with id {id} not found")
    {
    }
}

