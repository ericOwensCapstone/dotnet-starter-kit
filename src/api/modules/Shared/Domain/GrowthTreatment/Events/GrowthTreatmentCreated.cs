using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain.Events;
public sealed record GrowthTreatmentCreated : DomainEvent
{
    public GrowthTreatment? GrowthTreatment { get; set; }
}

