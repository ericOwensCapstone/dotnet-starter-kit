using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses.Exceptions;
using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Update.v1;
public sealed class UpdateContractStatusHandler(
    ILogger<UpdateContractStatusHandler> logger,
    [FromKeyedServices("ranch:contractStatuses")] IRepository<ContractStatus> repository)
    : IRequestHandler<UpdateContractStatusCommand, UpdateContractStatusResponse>
{
    public async Task<UpdateContractStatusResponse> Handle(UpdateContractStatusCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var contractStatus = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = contractStatus ?? throw new ContractStatusNotFoundException(request.Id);
        
        var updatedContractStatus = contractStatus.Update(
            request.TenantId,
            request.Name,
            request.Description
        );
        await repository.UpdateAsync(updatedContractStatus, cancellationToken);
        logger.LogInformation("contractStatus with id : {ContractStatusId} updated.", contractStatus.Id);
        return new UpdateContractStatusResponse(contractStatus.Id);
    }
}

