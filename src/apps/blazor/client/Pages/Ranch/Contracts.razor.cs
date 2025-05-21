using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FSH.Starter.Blazor.Client.Pages.Ranch;

public partial class Contracts
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<ContractResponse, Guid, ContractViewModel> Context { get; set; } = default!;

    private EntityTable<ContractResponse, Guid, ContractViewModel> _table = default!;

    //Start Subentity Lists    
    private List<MemberPageResponse> _memberPages = new();
    public MemberPageResponse SelectedMemberPage { get; set; } = default!;
    public List<ContractMemberPage> ApprovedMemberPages { get; set; } = new();
    public List<ContractResponse> CurrentPage { get; set; } = new();
    
    private List<ContractStatusResponse> _contractStatuses = new();
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
            entityName: "Contract",
            entityNamePlural: "Contracts",
            entityResource: FshResources.Contracts,
            fields: new() {
                new(contract => contract.Name, "Name", "Name"),
                new(contract => contract.Description, "Description", "Description"),
                new(contract => contract.ContractStatus.Name, "ContractStatus", "ContractStatus"),
            },
            enableAdvancedSearch: true,
            idFunc: contract => contract.Id!.Value,
            searchFunc: async filter =>
            {
                var contractFilter = filter.Adapt<SearchContractsCommand>();
                //Start filter parameters
                contractFilter.ContractStatusId = SearchContractStatusId;
                //End filter parameters
                var result = await _client.SearchContractsEndpointAsync("1", contractFilter);
                //Start Search Func List code
                CurrentPage = result.Items?.ToList<ContractResponse>();
                ApprovedMemberPages = new();
                //End Search Func List code
                return result.Adapt<PaginationResponse<ContractResponse>>();
            },
            createFunc: async contract =>
            {
                //Start Create Func code
                var createCommand = contract.Adapt<CreateContractCommand>();


                List<CreateContractMemberPageCommand> ContractMemberPageCommands = new List<CreateContractMemberPageCommand>();
                foreach (var item in ApprovedMemberPages)
                {
                    var createContractMemberPageCommand = new CreateContractMemberPageCommand
                    {
                        MemberPageId = item.MemberPage.Id,
                    };
                    ContractMemberPageCommands.Add(createContractMemberPageCommand);
                }
                
                createCommand.ContractMemberPages = ContractMemberPageCommands;
                
                await _client.CreateContractEndpointAsync("1", createCommand);
                //End Create Func code

                //Start One Per Create Func Code//End One Per Create Func Code
            },
            //Start Edit Func code
            editFormInitializedFunc: async () =>
            {
                await Task.Delay(1);
                ApprovedMemberPages = new();
                var temp = Context.AddEditModal.RequestModel;
                var target = CurrentPage.FirstOrDefault(lp => lp.Id == temp.Id);
                if (target != null)
                {
                    ApprovedMemberPages = new();
                        foreach (var item in target.ContractMemberPages)
                        {
                            var newMemberPageSelection = new ContractMemberPage
                            {
                                MemberPageId = item.MemberPageId,
                                MemberPage = item.MemberPage,
                            };
                            ApprovedMemberPages.Add(newMemberPageSelection);
                        }
    
                }
                Context.AddEditModal.ForceRender();
            },
            //End Edit Func code
            updateFunc: async (id, contract) =>
            {
                //Start Update Func code
                var updateCommand = contract.Adapt<UpdateContractCommand>();

                List<UpdateContractMemberPageCommand> ContractMemberPageCommands = new List<UpdateContractMemberPageCommand>();
                foreach (var item in ApprovedMemberPages)
                {
                    var updateContractMemberPageCommand = new UpdateContractMemberPageCommand
                    {
                        ContractId = id,
                        MemberPageId = item.MemberPage.Id,
                    };
                    ContractMemberPageCommands.Add(updateContractMemberPageCommand);
                }
                
                updateCommand.ContractMemberPages = ContractMemberPageCommands;
                
                await _client.UpdateContractEndpointAsync("1", id, updateCommand);
                //End Update Func code
            },
            deleteFunc: async id =>
            {
                //Start Delete Func code
                await _client.DeleteContractEndpointAsync("1", id);
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
        await LoadMemberPagesAsync();
        await LoadContractStatusesAsync();
        //End Subentity Loader calls
    }

    //Start List Related Code
    public void AddToMemberPageList()
    {
        var newSelection = new ContractMemberPage
        {
            MemberPageId = (Guid)SelectedMemberPage.Id,
            MemberPage = SelectedMemberPage.Adapt<MemberPage>(),
        };
        ApprovedMemberPages.Add(newSelection);
        SelectedMemberPage = null;
        Context.AddEditModal.ForceRender();
    }

    public void DeleteFromMemberPageList(ContractMemberPage deletedItem)
    {
        ApprovedMemberPages.Remove(deletedItem);
        Context.AddEditModal.ForceRender();
    }
    //End List Related Code

    //Start Subentity Loaders
    private async Task LoadMemberPagesAsync()
    {
        if (_memberPages.Count == 0)
        {
            var response = await _client.SearchMemberPagesEndpointAsync("1", new SearchMemberPagesCommand());
            if (response?.Items != null)
            {
                _memberPages = response.Items.ToList();
            }
        }
    }

    private async Task LoadContractStatusesAsync()
    {
        if (_contractStatuses.Count == 0)
        {
            var response = await _client.SearchContractStatusesEndpointAsync("1", new SearchContractStatusesCommand());
            if (response?.Items != null)
            {
                _contractStatuses = response.Items.ToList();
            }
        }
    }

    //End Subentity Loaders

    //Start Subentity Advanced Search

    private Guid? _searchContractStatusId;
    private Guid? SearchContractStatusId
    {
        get => _searchContractStatusId;
        set
        {
            _searchContractStatusId = value;
            _ = _table.ReloadDataAsync();
        }
    }
    //End Subentity Advanced Search

}

public class ContractViewModel : UpdateContractCommand
{
}
