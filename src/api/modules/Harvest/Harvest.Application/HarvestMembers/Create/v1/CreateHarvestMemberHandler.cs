using FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Search.v1;
using FSH.Framework.Infrastructure.Tenant;
using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Create.v1;
public sealed class CreateHarvestMemberHandler(
    //Start Injections
    IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor,
    //End Injections
    ILogger<CreateHarvestMemberHandler> logger,
    [FromKeyedServices("harvest:harvestMembers")] IRepository<HarvestMember> repository)
    : IRequestHandler<CreateHarvestMemberCommand, CreateHarvestMemberResponse>
{
    public async Task<CreateHarvestMemberResponse> Handle(CreateHarvestMemberCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        //Start One Per Code
        var currentTenantId = multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id ?? throw new InvalidOperationException("TenantId is not set.");
        var searchCommand = new SearchHarvestMembersCommand();
        searchCommand.TenantId = currentTenantId;
        var spec = new SearchHarvestMemberSpecs(searchCommand);
        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        if(items.Count > 0)
        {
            throw new InvalidOperationException("Tenant already has this entity.");
        }
        //End One Per Code

        var harvestMember = HarvestMember.Create(
            request.TenantId,
            request.MemberId,
            request.Name,
            request.Description
        );
        
        await repository.AddAsync(harvestMember, cancellationToken);
        logger.LogInformation("harvestMember created {HarvestMemberId}", harvestMember.Id);
        return new CreateHarvestMemberResponse(harvestMember.Id);
    }
}

