using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Shared;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace FSH.Starter.Blazor.Client.Pages.LifecycleProgramCatalog;

public partial class LifecyclePrograms
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<LifecycleProgramResponse, Guid, LifecycleProgramViewModel> Context { get; set; } = default!;

    private EntityTable<LifecycleProgramResponse, Guid, LifecycleProgramViewModel> _table = default!;

    private List<LifecycleStageResponse> LocalLifecycleStages { get; set; } = new();

    public LifecycleStageResponse SelectedLifecycleStage { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Context = new(
            entityName: "LifecycleProgram",
            entityNamePlural: "LifecyclePrograms",
            entityResource: FshResources.LifecyclePrograms,
            fields: new()
            {
                //new(prod => prod.Id,"Id", "Id"),
                new(prod => prod.Name,"Name", "Name"),
                new(prod => prod.Description, "Description", "Description"),
                new(prod => prod.Rating, "Rating", "Rating")
            },
            enableAdvancedSearch: true,
            idFunc: prod => prod.Id!.Value,
            searchFunc: async filter =>
            {
                var lifecycleProgramFilter = filter.Adapt<PaginationFilter>();

                var result = await _client.SearchLifecycleProgramsEndpointAsync("1", lifecycleProgramFilter);
                return result.Adapt<PaginationResponse<LifecycleProgramResponse>>();
            },
            createFunc: async prod =>
            {
                //TODO in lieu of actually selecting them for now
                //prod.LifecycleProgramStages = LocalLifecycleStages.Select(x => x.Adapt<UpdateLifecycleProgramStageCommand>()).ToList();

                //var lifecycleProgram = new LifecycleProgram 
                //{
                //    Name = prod.Name,
                //    Description = prod.Description,
                //    Rating = prod.Rating
                //};

                List<UpdateLifecycleProgramStageCommand> commands = new List<UpdateLifecycleProgramStageCommand>();
                var order = 0;
                foreach(var lifecycleStage in LocalLifecycleStages)
                {
                    var updateProgramStageCommand = new UpdateLifecycleProgramStageCommand
                    {
                        LifecycleStageId = (Guid)lifecycleStage.Id,
                        Order = order++
                    };
                    commands.Add(updateProgramStageCommand);
                }
                prod.UpdateLifecycleProgramStageCommands = commands;
                var createCommand = prod.Adapt<CreateLifecycleProgramCommand>();
                createCommand.UpdateLifeycleProgramStageCommands = commands;
                


                await _client.CreateLifecycleProgramEndpointAsync("1", createCommand);

            },
            updateFunc: async (id, prod) =>
            {
                List<UpdateLifecycleProgramStageCommand> commands = new List<UpdateLifecycleProgramStageCommand>();
                var order = 0;
                foreach (var lifecycleStage in LocalLifecycleStages)
                {
                    var updateProgramStageCommand = new UpdateLifecycleProgramStageCommand
                    {
                        LifecycleStageId = (Guid)lifecycleStage.Id,
                        Order = order++
                    };
                    commands.Add(updateProgramStageCommand);
                }
                prod.UpdateLifecycleProgramStageCommands = commands;
                var updateCommand = prod.Adapt<UpdateLifecycleProgramCommand>();
                updateCommand.UpdateLifecycleProgramStageCommands = commands;

                await _client.UpdateLifecycleProgramEndpointAsync("1", id, prod.Adapt<UpdateLifecycleProgramCommand>());
            },
            deleteFunc: async id => await _client.DeleteLifecycleProgramEndpointAsync("1", id));

        LocalLifecycleStages = await LoadLifecycleStagesAsync();
    }

    private async Task<List<LifecycleStageResponse>> LoadLifecycleStagesAsync()
    {
        var filter = new PaginationFilter { PageSize = 1000 };

        var result = await _client.SearchLifecycleStagesEndpointAsync("1", filter);
        if (result.Items == null)
        {
            return new();
        }
        return (List<LifecycleStageResponse>)result.Items;
    }

    public void OnSelectionChanged(IEnumerable<LifecycleStageResponse> selections)
    {
        // Your logic here
        //Console.WriteLine($"Selection changed to: {newValue}");
        var wd = 40;
    }

    // Advanced Search

    private Guid _searchBrandId;
    private Guid SearchBrandId
    {
        get => _searchBrandId;
        set
        {
            _searchBrandId = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private decimal _searchMinimumRate;
    private decimal SearchMinimumRate
    {
        get => _searchMinimumRate;
        set
        {
            _searchMinimumRate = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private decimal _searchMaximumRate = 9999;
    private decimal SearchMaximumRate
    {
        get => _searchMaximumRate;
        set
        {
            _searchMaximumRate = value;
            _ = _table.ReloadDataAsync();
        }
    }
}

public class LifecycleStageSelection
{
    public LifecycleStageResponse LifecycleStage { get; set; } = default!;
    public int Order { get; set; }
}
public class LifecycleProgramViewModel : UpdateLifecycleProgramCommand
{
    public LifecycleStageResponse SelectedLifecycleStage { get; set; } = default!;

    public List<LifecycleStageSelection> LifecycleStageSelections { get; set; } = new();
}
