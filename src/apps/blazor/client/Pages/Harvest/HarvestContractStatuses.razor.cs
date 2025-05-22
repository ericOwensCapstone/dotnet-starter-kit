using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.Starter.Blazor.Client.Pages.Harvest;

public partial class HarvestContractStatuses
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<HarvestContractStatusResponse, Guid, HarvestContractStatusViewModel> Context { get; set; } = default!;

    private EntityTable<HarvestContractStatusResponse, Guid, HarvestContractStatusViewModel> _table = default!;

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
            entityName: "HarvestContractStatus",
            entityNamePlural: "HarvestContractStatuses",
            entityResource: FshResources.HarvestContractStatuses,
            fields: new() {
                new(harvestContractStatus => harvestContractStatus.Name, "Name", "Name"),
                new(harvestContractStatus => harvestContractStatus.Description, "Description", "Description"),
            },
            enableAdvancedSearch: true,
            idFunc: harvestContractStatus => harvestContractStatus.Id!.Value,
            searchFunc: async filter =>
            {
                var harvestContractStatusFilter = filter.Adapt<SearchHarvestContractStatusesCommand>();
                //Start filter parameters
                //End filter parameters
                var result = await _client.SearchHarvestContractStatusesEndpointAsync("1", harvestContractStatusFilter);
                //Start Search Func List code
                //End Search Func List code
                return result.Adapt<PaginationResponse<HarvestContractStatusResponse>>();
            },
            createFunc: async harvestContractStatus =>
            {
                //Start Create Func code
                await _client.CreateHarvestContractStatusEndpointAsync("1", harvestContractStatus.Adapt<CreateHarvestContractStatusCommand>());
                //End Create Func code

                //Start One Per Create Func Code//End One Per Create Func Code
            },
            //Start Edit Func code//End Edit Func code
            updateFunc: async (id, harvestContractStatus) =>
            {
                //Start Update Func code
                await _client.UpdateHarvestContractStatusEndpointAsync("1", id, harvestContractStatus.Adapt<UpdateHarvestContractStatusCommand>());
                //End Update Func code
            },
            deleteFunc: async id =>
            {
                //Start Delete Func code
                await _client.DeleteHarvestContractStatusEndpointAsync("1", id);
                //End Delete Func code
                _canCreateIt = true;
            },
            canCreateEntityFunc: () => {
                return _canCreateIt;
            },
            canUpdateEntityFunc: ad => ad.TenantId == CurrentTenantId, 
            canDeleteEntityFunc: ad => ad.TenantId == CurrentTenantId  
        );

        //Start One Per Initialize Code//End One Per Initialize Code

        //Start Subentity Loader calls
        //End Subentity Loader calls
    }

    //Start List Related Code//End List Related Code

    //Start Subentity Loaders
    //End Subentity Loaders

    //Start Subentity Advanced Search
    //End Subentity Advanced Search

}

public class HarvestContractStatusViewModel : UpdateHarvestContractStatusCommand
{
}
