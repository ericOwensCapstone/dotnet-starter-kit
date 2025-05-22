using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers.Exceptions;
using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Update.v1;
public sealed class UpdateHarvestMemberHandler(
    ILogger<UpdateHarvestMemberHandler> logger,
    [FromKeyedServices("harvest:harvestMembers")] IRepository<HarvestMember> repository)
    : IRequestHandler<UpdateHarvestMemberCommand, UpdateHarvestMemberResponse>
{
    public async Task<UpdateHarvestMemberResponse> Handle(UpdateHarvestMemberCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var harvestMember = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = harvestMember ?? throw new HarvestMemberNotFoundException(request.Id);
        
        var updatedHarvestMember = harvestMember.Update(
            request.TenantId,
            request.MemberId,
            request.Name,
            request.Description
        );
        await repository.UpdateAsync(updatedHarvestMember, cancellationToken);
        logger.LogInformation("harvestMember with id : {HarvestMemberId} updated.", harvestMember.Id);
        return new UpdateHarvestMemberResponse(harvestMember.Id);
    }
}

