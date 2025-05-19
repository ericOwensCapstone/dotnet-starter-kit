using Mapster;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.Contracts.Exceptions;
using FSH.Starter.WebApi.Ranch.Domain.Contracts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Update.v1;
public sealed class UpdateContractHandler(
    ILogger<UpdateContractHandler> logger,
    [FromKeyedServices("ranch:contracts")] IRepository<Contract> repository)
    : IRequestHandler<UpdateContractCommand, UpdateContractResponse>
{
    public async Task<UpdateContractResponse> Handle(UpdateContractCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var contractMemberPages = new List<ContractMemberPage>();
        foreach (var v in request.ContractMemberPages)
        {
            var r = v.Adapt<ContractMemberPage>();
            contractMemberPages.Add(r);
        }

        var contract = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = contract ?? throw new ContractNotFoundException(request.Id);
        
        var updatedContract = contract.Update(
            request.TenantId,
            request.MemberId,
            contractMemberPages,
            request.Name,
            request.Description,
            request.ContractStatusId
        );
        await repository.UpdateAsync(updatedContract, cancellationToken);
        logger.LogInformation("contract with id : {ContractId} updated.", contract.Id);
        return new UpdateContractResponse(contract.Id);
    }
}

