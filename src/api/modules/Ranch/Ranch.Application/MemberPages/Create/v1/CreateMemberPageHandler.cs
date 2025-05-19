using FSH.Starter.WebApi.Ranch.Application.MemberPages.Search.v1;
using FSH.Framework.Infrastructure.Tenant;
using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.MemberPages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.MemberPages.Create.v1;
public sealed class CreateMemberPageHandler(
    //Start Injections
    IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor,
    //End Injections
    ILogger<CreateMemberPageHandler> logger,
    [FromKeyedServices("ranch:memberPages")] IRepository<MemberPage> repository)
    : IRequestHandler<CreateMemberPageCommand, CreateMemberPageResponse>
{
    public async Task<CreateMemberPageResponse> Handle(CreateMemberPageCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        //Start One Per Code
        var currentTenantId = multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id ?? throw new InvalidOperationException("TenantId is not set.");
        var searchCommand = new SearchMemberPagesCommand();
        searchCommand.TenantId = currentTenantId;
        var spec = new SearchMemberPageSpecs(searchCommand);
        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        if(items.Count > 0)
        {
            throw new InvalidOperationException("Tenant already has this entity.");
        }
        //End One Per Code

        var memberPage = MemberPage.Create(
            request.TenantId,
            request.MemberId,
            request.Name,
            request.Description
        );
        
        await repository.AddAsync(memberPage, cancellationToken);
        logger.LogInformation("memberPage created {MemberPageId}", memberPage.Id);
        return new CreateMemberPageResponse(memberPage.Id);
    }
}

