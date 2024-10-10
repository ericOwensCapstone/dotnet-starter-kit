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
                //CreateLifecycleProgramCommand command = null;
                ////TODO debug
                //try
                //{
                //    command = new CreateLifecycleProgramCommand
                //    {
                //        Name = prod.Name,
                //        Description = prod.Description,
                //        Rating = prod.Rating,
                //        LifecycleStages = LocalLifecycleStages.Select(x => x.Adapt<UpdateLifecycleStageCommand>()).ToList()
                //    };
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //}
                //await _client.CreateLifecycleProgramEndpointAsync("1", command);
                //TODO in lieu of actually selecting them for now
                prod.LifecycleStages = LocalLifecycleStages.Select(x => x.Adapt<UpdateLifecycleStageCommand>()).ToList();
                await _client.CreateLifecycleProgramEndpointAsync("1", prod.Adapt<CreateLifecycleProgramCommand>());

            },
            updateFunc: async (id, prod) =>
            {
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

public class LifecycleProgramViewModel : UpdateLifecycleProgramCommand
{
    //public List<LifecycleStageResponse> LifecycleStages { get; set; } = new();
    public List<UpdateLifecycleStageCommand> LifecycleStages { get; set; } = new();

}
