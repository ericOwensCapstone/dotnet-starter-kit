using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses.Exceptions;
using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Delete.v1;
public sealed class DeleteContractStatusHandler(
    ILogger<DeleteContractStatusHandler> logger,
    [FromKeyedServices("ranch:contractStatuses")] IRepository<ContractStatus> repository)
    : IRequestHandler<DeleteContractStatusCommand>
{
    public async Task Handle(DeleteContractStatusCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var contractStatus = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = contractStatus ?? throw new ContractStatusNotFoundException(request.Id);
        await repository.DeleteAsync(contractStatus, cancellationToken);
        logger.LogInformation("contractStatus with id : {ContractStatusId} deleted", contractStatus.Id);
    }
}

