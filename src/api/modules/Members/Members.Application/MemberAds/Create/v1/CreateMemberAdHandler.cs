using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Members.Domain.MemberAds;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Members.Application.MemberAds.Create.v1;
public sealed class CreateMemberAdHandler(
    ILogger<CreateMemberAdHandler> logger,
    [FromKeyedServices("members:memberAds")] IRepository<MemberAd> repository)
    : IRequestHandler<CreateMemberAdCommand, CreateMemberAdResponse>
{
    public async Task<CreateMemberAdResponse> Handle(CreateMemberAdCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var memberAd = MemberAd.Create(
            request.TenantId,
            request.Name,
            request.Description
        );
        await repository.AddAsync(memberAd, cancellationToken);
        logger.LogInformation("memberAd created {MemberAdId}", memberAd.Id);
        return new CreateMemberAdResponse(memberAd.Id);
    }
}

