using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Members.Domain.MemberAds.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using MediatR;
using FSH.Starter.WebApi.Members.Domain.MemberAds;

namespace FSH.Starter.WebApi.Members.Application.MemberAds.Get.v1;
public sealed class GetMemberAdHandler(
    [FromKeyedServices("members:memberAds")] IReadRepository<MemberAd> repository,
    ICacheService cache)
    : IRequestHandler<GetMemberAdRequest, MemberAdResponse>
{
    public async Task<MemberAdResponse> Handle(GetMemberAdRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"memberAd:{request.Id}",
            async () =>
            {
                var spec = new GetMemberAdSpecs(request.Id);
                var memberAdItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (memberAdItem == null) throw new MemberAdNotFoundException(request.Id);
                return memberAdItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

