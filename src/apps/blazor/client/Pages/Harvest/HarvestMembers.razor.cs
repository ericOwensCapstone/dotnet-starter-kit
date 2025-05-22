using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.Starter.Blazor.Client.Pages.Harvest;

public partial class HarvestMembers
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<HarvestMemberResponse, Guid, HarvestMemberViewModel> Context { get; set; } = default!;

    private EntityTable<HarvestMemberResponse, Guid, HarvestMemberViewModel> _table = default!;

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
            entityName: "HarvestMember",
            entityNamePlural: "HarvestMembers",
            entityResource: FshResources.HarvestMembers,
            fields: new() {
                new(harvestMember => harvestMember.Name, "Name", "Name"),
                new(harvestMember => harvestMember.Description, "Description", "Description"),
            },
            enableAdvancedSearch: true,
            idFunc: harvestMember => harvestMember.Id!.Value,
            searchFunc: async filter =>
            {
                var harvestMemberFilter = filter.Adapt<SearchHarvestMembersCommand>();
                //Start filter parameters
                //End filter parameters
                var result = await _client.SearchHarvestMembersEndpointAsync("1", harvestMemberFilter);
                //Start Search Func List code
                //End Search Func List code
                return result.Adapt<PaginationResponse<HarvestMemberResponse>>();
            },
            createFunc: async harvestMember =>
            {
                //Start Create Func code
                await _client.CreateHarvestMemberEndpointAsync("1", harvestMember.Adapt<CreateHarvestMemberCommand>());
                //End Create Func code

                //Start One Per Create Func Code
                _canCreateIt = false;
                //End One Per Create Func Code
            },
            //Start Edit Func code//End Edit Func code
            updateFunc: async (id, harvestMember) =>
            {
                //Start Update Func code
                await _client.UpdateHarvestMemberEndpointAsync("1", id, harvestMember.Adapt<UpdateHarvestMemberCommand>());
                //End Update Func code
            },
            deleteFunc: async id =>
            {
                //Start Delete Func code
                await _client.DeleteHarvestMemberEndpointAsync("1", id);
                //End Delete Func code
                _canCreateIt = true;
            },
            canCreateEntityFunc: () => {
                return _canCreateIt;
            },
            canUpdateEntityFunc: ad => ad.TenantId == CurrentTenantId, 
            canDeleteEntityFunc: ad => ad.TenantId == CurrentTenantId  
        );

        //Start One Per Initialize Code
        var filter = new SearchHarvestMembersCommand();
        filter.TenantId = CurrentTenantId;
        var harvestMembers = await _client.SearchHarvestMembersEndpointAsync("1", filter);
        if (harvestMembers.TotalCount != 0)
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

public class HarvestMemberViewModel : UpdateHarvestMemberCommand
{
}
