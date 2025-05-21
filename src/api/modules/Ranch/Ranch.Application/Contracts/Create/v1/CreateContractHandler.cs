using Mapster;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.Contracts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Create.v1;
public sealed class CreateContractHandler(
    //Start Injections//End Injections
    ILogger<CreateContractHandler> logger,
    [FromKeyedServices("ranch:contracts")] IRepository<Contract> repository)
    : IRequestHandler<CreateContractCommand, CreateContractResponse>
{
    public async Task<CreateContractResponse> Handle(CreateContractCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        //Start One Per Code//End One Per Code

        var contractMemberPages = new List<ContractMemberPage>();
        foreach (var v in request.ContractMemberPages)
        {
            var r = v.Adapt<ContractMemberPage>();
            contractMemberPages.Add(r);
        }

        var contract = Contract.Create(
            request.TenantId,
            request.MemberId,
            contractMemberPages,
            request.Name,
            request.Description,
            request.ContractStatusId
        );
        
        await repository.AddAsync(contract, cancellationToken);
        logger.LogInformation("contract created {ContractId}", contract.Id);
        return new CreateContractResponse(contract.Id);
    }
}

