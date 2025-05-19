using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Ranch.Domain.MemberPages.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using MediatR;
using FSH.Starter.WebApi.Ranch.Domain.MemberPages;

namespace FSH.Starter.WebApi.Ranch.Application.MemberPages.Get.v1;
public sealed class GetMemberPageHandler(
    [FromKeyedServices("ranch:memberPages")] IReadRepository<MemberPage> repository,
    ICacheService cache)
    : IRequestHandler<GetMemberPageRequest, MemberPageResponse>
{
    public async Task<MemberPageResponse> Handle(GetMemberPageRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"memberPage:{request.Id}",
            async () =>
            {
                var spec = new GetMemberPageSpecs(request.Id);
                var memberPageItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (memberPageItem == null) throw new MemberPageNotFoundException(request.Id);
                return memberPageItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

