using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.MemberPages.Exceptions;
using FSH.Starter.WebApi.Ranch.Domain.MemberPages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.MemberPages.Update.v1;
public sealed class UpdateMemberPageHandler(
    ILogger<UpdateMemberPageHandler> logger,
    [FromKeyedServices("ranch:memberPages")] IRepository<MemberPage> repository)
    : IRequestHandler<UpdateMemberPageCommand, UpdateMemberPageResponse>
{
    public async Task<UpdateMemberPageResponse> Handle(UpdateMemberPageCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var memberPage = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = memberPage ?? throw new MemberPageNotFoundException(request.Id);
        
        var updatedMemberPage = memberPage.Update(
            request.TenantId,
            request.MemberId,
            request.Name,
            request.Description
        );
        await repository.UpdateAsync(updatedMemberPage, cancellationToken);
        logger.LogInformation("memberPage with id : {MemberPageId} updated.", memberPage.Id);
        return new UpdateMemberPageResponse(memberPage.Id);
    }
}

