using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Create.v1;
public sealed class CreateContractStatusHandler(
    ILogger<CreateContractStatusHandler> logger,
    [FromKeyedServices("ranch:contractStatuses")] IRepository<ContractStatus> repository)
    : IRequestHandler<CreateContractStatusCommand, CreateContractStatusResponse>
{
    public async Task<CreateContractStatusResponse> Handle(CreateContractStatusCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var contractStatus = ContractStatus.Create(
            request.TenantId,
            request.Name,
            request.Description
        );
        await repository.AddAsync(contractStatus, cancellationToken);
        logger.LogInformation("contractStatus created {ContractStatusId}", contractStatus.Id);
        return new CreateContractStatusResponse(contractStatus.Id);
    }
}

