using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.Starter.Blazor.Client.Pages.Ranch;

public partial class ContractStatuses
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<ContractStatusResponse, Guid, ContractStatusViewModel> Context { get; set; } = default!;

    private EntityTable<ContractStatusResponse, Guid, ContractStatusViewModel> _table = default!;

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
            entityName: "ContractStatus",
            entityNamePlural: "ContractStatuses",
            entityResource: FshResources.ContractStatuses,
            fields: new() {
                new(contractStatus => contractStatus.Name, "Name", "Name"),
                new(contractStatus => contractStatus.Description, "Description", "Description"),
            },
            enableAdvancedSearch: true,
            idFunc: contractStatus => contractStatus.Id!.Value,
            searchFunc: async filter =>
            {
                var contractStatusFilter = filter.Adapt<SearchContractStatusesCommand>();
                //Start filter parameters
                //End filter parameters
                var result = await _client.SearchContractStatusesEndpointAsync("1", contractStatusFilter);
                //Start Search Func List code
                //End Search Func List code
                return result.Adapt<PaginationResponse<ContractStatusResponse>>();
            },
            createFunc: async contractStatus =>
            {
                //Start Create Func code
                await _client.CreateContractStatusEndpointAsync("1", contractStatus.Adapt<CreateContractStatusCommand>());
                //End Create Func code

                //Start One Per Create Func Code//End One Per Create Func Code
            },
            //Start Edit Func code//End Edit Func code
            updateFunc: async (id, contractStatus) =>
            {
                //Start Update Func code
                await _client.UpdateContractStatusEndpointAsync("1", id, contractStatus.Adapt<UpdateContractStatusCommand>());
                //End Update Func code
            },
            deleteFunc: async id =>
            {
                //Start Delete Func code
                await _client.DeleteContractStatusEndpointAsync("1", id);
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

public class ContractStatusViewModel : UpdateContractStatusCommand
{
}
