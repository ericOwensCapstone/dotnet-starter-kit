using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using MediatR;
using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Get.v1;
public sealed class GetHarvestMemberHandler(
    [FromKeyedServices("harvest:harvestMembers")] IReadRepository<HarvestMember> repository,
    ICacheService cache)
    : IRequestHandler<GetHarvestMemberRequest, HarvestMemberResponse>
{
    public async Task<HarvestMemberResponse> Handle(GetHarvestMemberRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"harvestMember:{request.Id}",
            async () =>
            {
                var spec = new GetHarvestMemberSpecs(request.Id);
                var harvestMemberItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (harvestMemberItem == null) throw new HarvestMemberNotFoundException(request.Id);
                return harvestMemberItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

