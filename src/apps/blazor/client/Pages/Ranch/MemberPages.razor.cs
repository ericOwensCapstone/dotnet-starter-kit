using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.Starter.Blazor.Client.Pages.Ranch;

public partial class MemberPages
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<MemberPageResponse, Guid, MemberPageViewModel> Context { get; set; } = default!;

    private EntityTable<MemberPageResponse, Guid, MemberPageViewModel> _table = default!;

    //Start Subentity Lists
    //End Subentity Lists

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    private string CurrentTenantId { get; set; } = default!;
    private bool _canCreateIt = true;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState;
        CurrentTenantId = authState.User.FindFirst("tenant")?.Value ?? string.Empty;

        Context = new(
            entityName: "MemberPage",
            entityNamePlural: "MemberPages",
            entityResource: FshResources.MemberPages,
            fields: new() {
                new(memberPage => memberPage.Name, "Name", "Name"),
                new(memberPage => memberPage.Description, "Description", "Description"),
            },
            enableAdvancedSearch: true,
            idFunc: memberPage => memberPage.Id!.Value,
            searchFunc: async filter =>
            {
                var memberPageFilter = filter.Adapt<SearchMemberPagesCommand>();
                //Start filter parameters
                //End filter parameters
                var result = await _client.SearchMemberPagesEndpointAsync("1", memberPageFilter);
                //Start Search Func List code
                //End Search Func List code
                return result.Adapt<PaginationResponse<MemberPageResponse>>();
            },
            createFunc: async memberPage =>
            {
                //Start Create Func code
                await _client.CreateMemberPageEndpointAsync("1", memberPage.Adapt<CreateMemberPageCommand>());
                //End Create Func code

                //Start One Per Create Func Code
                _canCreateIt = false;
                //End One Per Create Func Code
            },
            //Start Edit Func code//End Edit Func code
            updateFunc: async (id, memberPage) =>
            {
                //Start Update Func code
                await _client.UpdateMemberPageEndpointAsync("1", id, memberPage.Adapt<UpdateMemberPageCommand>());
                //End Update Func code
            },
            deleteFunc: async id =>
            {
                //Start Delete Func code
                await _client.DeleteMemberPageEndpointAsync("1", id);
                //End Delete Func code
                _canCreateIt = true;
            },
            canCreateEntityFunc: () => {
                return _canCreateIt;
            },
            canUpdateEntityFunc: ad => ad.TenantId == CurrentTenantId, 
            canDeleteEntityFunc: ad => CurrentTenantId == "root" // Only root tenant can delete MemberPages
        );

        //Start One Per Initialize Code
        var filter = new SearchMemberPagesCommand();
        filter.TenantId = CurrentTenantId;
        var memberPages = await _client.SearchMemberPagesEndpointAsync("1", filter);
        if (memberPages.TotalCount != 0)
        {
            _canCreateIt = false;
        }        
        //End One Per Initialize Code

        //Start Subentity Loader calls
        //End Subentity Loader calls
    }

    //Start List Related Code//End List Related Code

    //Start Subentity Loaders
    //End Subentity Loaders

    //Start Subentity Advanced Search
    //End Subentity Advanced Search

}

public class MemberPageViewModel : UpdateMemberPageCommand
{
}
