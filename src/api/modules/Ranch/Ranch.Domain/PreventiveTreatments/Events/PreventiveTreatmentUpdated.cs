using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments;

namespace FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments.Events;
public sealed record PreventiveTreatmentUpdated : DomainEvent
{
    public PreventiveTreatment? PreventiveTreatment { get; set; }
}

