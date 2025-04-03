using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace FSH.Starter.Blazor.Client.Pages.Ranch;

public partial class LifecyclePrograms
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<LifecycleProgramResponse, Guid, LifecycleProgramViewModel> Context { get; set; } = default!;

    private EntityTable<LifecycleProgramResponse, Guid, LifecycleProgramViewModel> _table = default!;

    //Start Subentity Lists    
    private List<LifecycleStageResponse> _lifecycleStages = new();
    public LifecycleStageResponse SelectedLifecycleStage { get; set; } = default!;
    public int SelectedLifecycleStageOrder { get; set; }
    public List<LifecycleProgramLifecycleStage> ApprovedLifecycleStages { get; set; } = new();
    public List<LifecycleProgramResponse> CurrentPage { get; set; } = new();

    //End Subentity Lists

    protected override async Task OnInitializedAsync()
    {
        Context = new(
            entityName: "LifecycleProgram",
            entityNamePlural: "LifecyclePrograms",
            entityResource: FshResources.LifecyclePrograms,
            fields: new() {
                new(lifecycleProgram => lifecycleProgram.Name, "Name", "Name"),
                new(lifecycleProgram => lifecycleProgram.Description, "Description", "Description"),
            },
            enableAdvancedSearch: true,
            idFunc: lifecycleProgram => lifecycleProgram.Id!.Value,
            searchFunc: async filter =>
            {
                var lifecycleProgramFilter = filter.Adapt<SearchLifecycleProgramsCommand>();
                //Start filter parameters
                //End filter parameters
                var result = await _client.SearchLifecycleProgramsEndpointAsync("1", lifecycleProgramFilter);
                //Start Search Func List code
                CurrentPage = result.Items?.ToList<LifecycleProgramResponse>();
                ApprovedLifecycleStages = new();
                //End Search Func List code
                return result.Adapt<PaginationResponse<LifecycleProgramResponse>>();
            },
            createFunc: async lifecycleProgram =>
            {
                //Start Create Func code
                var createCommand = lifecycleProgram.Adapt<CreateLifecycleProgramCommand>();

                List<CreateLifecycleProgramLifecycleStageCommand> lifecycleProgramLifecycleStageCommands = new List<CreateLifecycleProgramLifecycleStageCommand>();
                foreach (var item in ApprovedLifecycleStages)
                {
                    var createLifecycleProgramLifecycleStageCommand = new CreateLifecycleProgramLifecycleStageCommand
                    {
                        LifecycleStageId = item.LifecycleStage.Id,
                        Order = item.Order
                    };
                    lifecycleProgramLifecycleStageCommands.Add(createLifecycleProgramLifecycleStageCommand);
                }
                
                createCommand.LifecycleProgramLifecycleStages = lifecycleProgramLifecycleStageCommands;
                
                await _client.CreateLifecycleProgramEndpointAsync("1", createCommand);
                //End Create Func code
            },
            //Start Edit Func code
            editFormInitializedFunc: async () =>
            {
                await Task.Delay(1);
                ApprovedLifecycleStages = new();
                var temp = Context.AddEditModal.RequestModel;
                var target = CurrentPage.FirstOrDefault(lp => lp.Id == temp.Id);
                if (target != null)
                {
                    ApprovedLifecycleStages = new();
                    foreach (var item in target.LifecycleProgramLifecycleStages)
                    {
                        var newLifecycleStageSelection = new LifecycleProgramLifecycleStage
                        {
                            LifecycleStageId = item.LifecycleStageId,
                            LifecycleStage = item.LifecycleStage,
                            Order = item.Order
                        };
                        ApprovedLifecycleStages.Add(newLifecycleStageSelection);
                    }
                    SortApprovedLifecycleStages();
    
                }
                Context.AddEditModal.ForceRender();
            },
            //End Edit Func code
            updateFunc: async (id, lifecycleProgram) =>
            {
                //Start Update Func code
                var updateCommand = lifecycleProgram.Adapt<UpdateLifecycleProgramCommand>();

                List<UpdateLifecycleProgramLifecycleStageCommand> lifecycleProgramLifecycleStageCommands = new List<UpdateLifecycleProgramLifecycleStageCommand>();
                foreach (var item in ApprovedLifecycleStages)
                {
                    var updateLifecycleProgramLifecycleStageCommand = new UpdateLifecycleProgramLifecycleStageCommand
                    {
                        LifecycleProgramId = id,
                        LifecycleStageId = item.LifecycleStage.Id,
                        Order = item.Order
                    };
                    lifecycleProgramLifecycleStageCommands.Add(updateLifecycleProgramLifecycleStageCommand);
                }
                
                updateCommand.LifecycleProgramLifecycleStages = lifecycleProgramLifecycleStageCommands;
                
                await _client.UpdateLifecycleProgramEndpointAsync("1", id, updateCommand);
                //End Update Func code
            },
            deleteFunc: async id => await _client.DeleteLifecycleProgramEndpointAsync("1", id));

        //Start Subentity Loader calls
        await LoadLifecycleStagesAsync();
        //End Subentity Loader calls
    }

    //Start List Related Code
    public void AddToLifecycleStageList()
    {
        var newSelection = new LifecycleProgramLifecycleStage
        {
            LifecycleStageId = (Guid)SelectedLifecycleStage.Id,
            LifecycleStage = SelectedLifecycleStage.Adapt<LifecycleStage>(),
            Order = SelectedLifecycleStageOrder
        };
        ApprovedLifecycleStages.Add(newSelection);
        SortApprovedLifecycleStages();
        SelectedLifecycleStage = null;
        SelectedLifecycleStageOrder = 0;
        Context.AddEditModal.ForceRender();
    }

    private void SortApprovedLifecycleStages()
    {
        ApprovedLifecycleStages = ApprovedLifecycleStages.OrderBy(item => item.Order).ToList();
    }

    public void DeleteFromLifecycleStageList(LifecycleProgramLifecycleStage deletedItem)
    {
        ApprovedLifecycleStages.Remove(deletedItem);
        SortApprovedLifecycleStages();
        Context.AddEditModal.ForceRender();
    }
    //End List Related Code

    //Start Subentity Loaders
    private async Task LoadLifecycleStagesAsync()
    {
        if (_lifecycleStages.Count == 0)
        {
            var response = await _client.SearchLifecycleStagesEndpointAsync("1", new SearchLifecycleStagesCommand());
            if (response?.Items != null)
            {
                _lifecycleStages = response.Items.ToList();
            }
        }
    }

    //End Subentity Loaders

    //Start Subentity Advanced Search
    //End Subentity Advanced Search

}

public class LifecycleProgramViewModel : UpdateLifecycleProgramCommand
{
}
