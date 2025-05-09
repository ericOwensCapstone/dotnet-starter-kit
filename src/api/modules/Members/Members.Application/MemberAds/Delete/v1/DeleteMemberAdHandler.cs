using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Members.Domain.MemberAds.Exceptions;
using FSH.Starter.WebApi.Members.Domain.MemberAds;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Members.Application.MemberAds.Delete.v1;
public sealed class DeleteMemberAdHandler(
    ILogger<DeleteMemberAdHandler> logger,
    [FromKeyedServices("members:memberAds")] IRepository<MemberAd> repository)
    : IRequestHandler<DeleteMemberAdCommand>
{
    public async Task Handle(DeleteMemberAdCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var memberAd = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = memberAd ?? throw new MemberAdNotFoundException(request.Id);
        await repository.DeleteAsync(memberAd, cancellationToken);
        logger.LogInformation("memberAd with id : {MemberAdId} deleted", memberAd.Id);
    }
}

