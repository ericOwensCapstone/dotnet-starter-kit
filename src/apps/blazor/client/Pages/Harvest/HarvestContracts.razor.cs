using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.Starter.Blazor.Client.Pages.Harvest;

public partial class HarvestContracts
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<HarvestContractResponse, Guid, HarvestContractViewModel> Context { get; set; } = default!;

    private EntityTable<HarvestContractResponse, Guid, HarvestContractViewModel> _table = default!;

    //Start Subentity Lists    
    private List<HarvestMemberResponse> _harvestMembers = new();
    public HarvestMemberResponse SelectedHarvestMember { get; set; } = default!;
    public List<HarvestContractHarvestMember> ApprovedHarvestMembers { get; set; } = new();
    public List<HarvestContractResponse> CurrentPage { get; set; } = new();
    
    private List<HarvestContractStatusResponse> _harvestContractStatuses = new();
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
            entityName: "HarvestContract",
            entityNamePlural: "HarvestContracts",
            entityResource: FshResources.HarvestContracts,
            fields: new() {
                new(harvestContract => harvestContract.Name, "Name", "Name"),
                new(harvestContract => harvestContract.Description, "Description", "Description"),
                new(harvestContract => harvestContract.HarvestContractStatus.Name, "HarvestContractStatus", "HarvestContractStatus"),
            },
            enableAdvancedSearch: true,
            idFunc: harvestContract => harvestContract.Id!.Value,
            searchFunc: async filter =>
            {
                var harvestContractFilter = filter.Adapt<SearchHarvestContractsCommand>();
                //Start filter parameters
                harvestContractFilter.HarvestContractStatusId = SearchHarvestContractStatusId;
                //End filter parameters
                var result = await _client.SearchHarvestContractsEndpointAsync("1", harvestContractFilter);
                //Start Search Func List code
                CurrentPage = result.Items?.ToList<HarvestContractResponse>();
                ApprovedHarvestMembers = new();
                //End Search Func List code
                return result.Adapt<PaginationResponse<HarvestContractResponse>>();
            },
            createFunc: async harvestContract =>
            {
                //Start Create Func code
                var createCommand = harvestContract.Adapt<CreateHarvestContractCommand>();


                List<CreateHarvestContractHarvestMemberCommand> HarvestContractHarvestMemberCommands = new List<CreateHarvestContractHarvestMemberCommand>();
                foreach (var item in ApprovedHarvestMembers)
                {
                    var createHarvestContractHarvestMemberCommand = new CreateHarvestContractHarvestMemberCommand
                    {
                        HarvestMemberId = item.HarvestMember.Id,
                    };
                    HarvestContractHarvestMemberCommands.Add(createHarvestContractHarvestMemberCommand);
                }
                
                createCommand.HarvestContractHarvestMembers = HarvestContractHarvestMemberCommands;
                
                await _client.CreateHarvestContractEndpointAsync("1", createCommand);
                //End Create Func code

                //Start One Per Create Func Code//End One Per Create Func Code
            },
            //Start Edit Func code
            editFormInitializedFunc: async () =>
            {
                await Task.Delay(1);
                ApprovedHarvestMembers = new();
                var temp = Context.AddEditModal.RequestModel;
                var target = CurrentPage.FirstOrDefault(lp => lp.Id == temp.Id);
                if (target != null)
                {
                    ApprovedHarvestMembers = new();
                        foreach (var item in target.HarvestContractHarvestMembers)
                        {
                            var newHarvestMemberSelection = new HarvestContractHarvestMember
                            {
                                HarvestMemberId = item.HarvestMemberId,
                                HarvestMember = item.HarvestMember,
                            };
                            ApprovedHarvestMembers.Add(newHarvestMemberSelection);
                        }
    
                }
                Context.AddEditModal.ForceRender();
            },
            //End Edit Func code
            updateFunc: async (id, harvestContract) =>
            {
                //Start Update Func code
                var updateCommand = harvestContract.Adapt<UpdateHarvestContractCommand>();

                List<UpdateHarvestContractHarvestMemberCommand> HarvestContractHarvestMemberCommands = new List<UpdateHarvestContractHarvestMemberCommand>();
                foreach (var item in ApprovedHarvestMembers)
                {
                    var updateHarvestContractHarvestMemberCommand = new UpdateHarvestContractHarvestMemberCommand
                    {
                        HarvestContractId = id,
                        HarvestMemberId = item.HarvestMember.Id,
                    };
                    HarvestContractHarvestMemberCommands.Add(updateHarvestContractHarvestMemberCommand);
                }
                
                updateCommand.HarvestContractHarvestMembers = HarvestContractHarvestMemberCommands;
                
                await _client.UpdateHarvestContractEndpointAsync("1", id, updateCommand);
                //End Update Func code
            },
            deleteFunc: async id =>
            {
                //Start Delete Func code
                await _client.DeleteHarvestContractEndpointAsync("1", id);
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
        await LoadHarvestMembersAsync();
        await LoadHarvestContractStatusesAsync();
        //End Subentity Loader calls
    }

    //Start List Related Code
    public void AddToHarvestMemberList()
    {
        var newSelection = new HarvestContractHarvestMember
        {
            HarvestMemberId = (Guid)SelectedHarvestMember.Id,
            HarvestMember = SelectedHarvestMember.Adapt<HarvestMember>(),
        };
        ApprovedHarvestMembers.Add(newSelection);
        SelectedHarvestMember = null;
        Context.AddEditModal.ForceRender();
    }

    public void DeleteFromHarvestMemberList(HarvestContractHarvestMember deletedItem)
    {
        ApprovedHarvestMembers.Remove(deletedItem);
        Context.AddEditModal.ForceRender();
    }
    //End List Related Code

    //Start Subentity Loaders
    private async Task LoadHarvestMembersAsync()
    {
        if (_harvestMembers.Count == 0)
        {
            var response = await _client.SearchHarvestMembersEndpointAsync("1", new SearchHarvestMembersCommand());
            if (response?.Items != null)
            {
                _harvestMembers = response.Items.ToList();
            }
        }
    }

    private async Task LoadHarvestContractStatusesAsync()
    {
        if (_harvestContractStatuses.Count == 0)
        {
            var response = await _client.SearchHarvestContractStatusesEndpointAsync("1", new SearchHarvestContractStatusesCommand());
            if (response?.Items != null)
            {
                _harvestContractStatuses = response.Items.ToList();
            }
        }
    }

    //End Subentity Loaders

    //Start Subentity Advanced Search

    private Guid? _searchHarvestContractStatusId;
    private Guid? SearchHarvestContractStatusId
    {
        get => _searchHarvestContractStatusId;
        set
        {
            _searchHarvestContractStatusId = value;
            _ = _table.ReloadDataAsync();
        }
    }
    //End Subentity Advanced Search

}

public class HarvestContractViewModel : UpdateHarvestContractCommand
{
}
