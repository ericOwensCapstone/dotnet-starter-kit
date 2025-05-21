using FSH.Framework.Core.Domain.Events;
using FSH.Starter.WebApi.Ranch.Domain.Contracts;

namespace FSH.Starter.WebApi.Ranch.Domain.Contracts.Events;
public sealed record ContractCreated : DomainEvent
{
    public Contract? Contract { get; set; }
}

