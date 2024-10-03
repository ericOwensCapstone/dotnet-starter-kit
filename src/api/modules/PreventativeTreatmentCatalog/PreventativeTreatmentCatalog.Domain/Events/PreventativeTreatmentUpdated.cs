using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain.Events;
public sealed record PreventativeTreatmentUpdated : DomainEvent
{
    public PreventativeTreatment? PreventativeTreatment { get; set; }
}

