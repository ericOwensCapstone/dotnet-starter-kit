using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Ranch.Domain.ContractStatuses.Exceptions;
public sealed class ContractStatusNotFoundException : NotFoundException
{
    public ContractStatusNotFoundException(Guid id)
        : base($"contractStatus with id {id} not found")
    {
    }
}

