using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Members.Domain.MemberAds.Exceptions;
using FSH.Starter.WebApi.Members.Domain.MemberAds;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Members.Application.MemberAds.Update.v1;
public sealed class UpdateMemberAdHandler(
    ILogger<UpdateMemberAdHandler> logger,
    [FromKeyedServices("members:memberAds")] IRepository<MemberAd> repository)
    : IRequestHandler<UpdateMemberAdCommand, UpdateMemberAdResponse>
{
    public async Task<UpdateMemberAdResponse> Handle(UpdateMemberAdCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var memberAd = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = memberAd ?? throw new MemberAdNotFoundException(request.Id);
        
        var updatedMemberAd = memberAd.Update(
            request.TenantId,
            request.Name,
            request.Description
        );
        await repository.UpdateAsync(updatedMemberAd, cancellationToken);
        logger.LogInformation("memberAd with id : {MemberAdId} updated.", memberAd.Id);
        return new UpdateMemberAdResponse(memberAd.Id);
    }
}

