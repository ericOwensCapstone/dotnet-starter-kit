using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers.Exceptions;
using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Delete.v1;
public sealed class DeleteHarvestMemberHandler(
    ILogger<DeleteHarvestMemberHandler> logger,
    [FromKeyedServices("harvest:harvestMembers")] IRepository<HarvestMember> repository)
    : IRequestHandler<DeleteHarvestMemberCommand>
{
    public async Task Handle(DeleteHarvestMemberCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var harvestMember = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = harvestMember ?? throw new HarvestMemberNotFoundException(request.Id);
        await repository.DeleteAsync(harvestMember, cancellationToken);
        logger.LogInformation("harvestMember with id : {HarvestMemberId} deleted", harvestMember.Id);
    }
}

