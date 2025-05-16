using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Ranch.Domain.Contracts.Exceptions;
public sealed class ContractNotFoundException : NotFoundException
{
    public ContractNotFoundException(Guid id)
        : base($"contract with id {id} not found")
    {
    }
}

