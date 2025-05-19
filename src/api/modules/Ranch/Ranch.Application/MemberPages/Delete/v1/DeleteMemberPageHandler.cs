using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.MemberPages.Exceptions;
using FSH.Starter.WebApi.Ranch.Domain.MemberPages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.MemberPages.Delete.v1;
public sealed class DeleteMemberPageHandler(
    ILogger<DeleteMemberPageHandler> logger,
    [FromKeyedServices("ranch:memberPages")] IRepository<MemberPage> repository)
    : IRequestHandler<DeleteMemberPageCommand>
{
    public async Task Handle(DeleteMemberPageCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var memberPage = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = memberPage ?? throw new MemberPageNotFoundException(request.Id);
        await repository.DeleteAsync(memberPage, cancellationToken);
        logger.LogInformation("memberPage with id : {MemberPageId} deleted", memberPage.Id);
    }
}

