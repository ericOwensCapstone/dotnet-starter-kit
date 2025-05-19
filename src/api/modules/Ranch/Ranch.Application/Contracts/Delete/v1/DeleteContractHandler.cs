using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.Contracts.Exceptions;
using FSH.Starter.WebApi.Ranch.Domain.Contracts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Delete.v1;
public sealed class DeleteContractHandler(
    ILogger<DeleteContractHandler> logger,
    [FromKeyedServices("ranch:contracts")] IRepository<Contract> repository)
    : IRequestHandler<DeleteContractCommand>
{
    public async Task Handle(DeleteContractCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var contract = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = contract ?? throw new ContractNotFoundException(request.Id);
        await repository.DeleteAsync(contract, cancellationToken);
        logger.LogInformation("contract with id : {ContractId} deleted", contract.Id);
    }
}

