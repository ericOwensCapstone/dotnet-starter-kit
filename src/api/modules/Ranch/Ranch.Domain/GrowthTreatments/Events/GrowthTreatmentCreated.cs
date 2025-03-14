using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Ranch.Domain.GrowthTreatments;

namespace FSH.Starter.WebApi.Ranch.Domain.GrowthTreatments.Events;
public sealed record GrowthTreatmentCreated : DomainEvent
{
    public GrowthTreatment? GrowthTreatment { get; set; }
}

