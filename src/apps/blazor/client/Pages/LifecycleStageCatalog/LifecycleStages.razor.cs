using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Shared;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace FSH.Starter.Blazor.Client.Pages.LifecycleStageCatalog;

public partial class LifecycleStages
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<LifecycleStageResponse, Guid, LifecycleStageViewModel> Context { get; set; } = default!;

    private EntityTable<LifecycleStageResponse, Guid, LifecycleStageViewModel> _table = default!;

    private List<RationResponse> Rations { get; set; } = new();
    private List<GrowthTreatmentResponse> GrowthTreatments { get; set; } = new();
    private List<PreventativeTreatmentResponse> PreventativeTreatments { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        Context = new(
            entityName: "LifecycleStage",
            entityNamePlural: "LifecycleStages",
            entityResource: FshResources.LifecycleStages,
            fields: new()
            {
                new(prod => prod.Id,"Id", "Id"),
                new(prod => prod.Name,"Name", "Name"),
                new(prod => prod.Description, "Description", "Description"),
                new(prod => prod.Ration.Name, "Ration", "Ration"),
                new(prod => prod.GrowthTreatment.Name, "Growth Treatment", "Growth Treatment"),
                new(prod => prod.PreventativeTreatment.Name, "Preventative Treatment", "Preventative Treatment"),

                //new(prod => prod.Rating, "Rating", "Rating")
            },
            enableAdvancedSearch: true,
            idFunc: prod => prod.Id!.Value,
            searchFunc: async filter =>
            {
                var lifecycleStageFilter = filter.Adapt<PaginationFilter>();

                var result = await _client.SearchLifecycleStagesEndpointAsync("1", lifecycleStageFilter);
                return result.Adapt<PaginationResponse<LifecycleStageResponse>>();
            },
            createFunc: async prod =>
            {
                await _client.CreateLifecycleStageEndpointAsync("1", prod.Adapt<CreateLifecycleStageCommand>());
            },
            updateFunc: async (id, prod) =>
            {
                await _client.UpdateLifecycleStageEndpointAsync("1", id, prod.Adapt<UpdateLifecycleStageCommand>());
            },
            deleteFunc: async id => await _client.DeleteLifecycleStageEndpointAsync("1", id));

        Rations = await LoadRationsAsync();
        GrowthTreatments = await LoadGrowthTreatmentsAsync();
        PreventativeTreatments = await LoadPreventativeTreatmentsAsync();
    }

    private async Task<List<RationResponse>> LoadRationsAsync()
    {
        var filter = new PaginationFilter { PageSize = 1000 };

        var result = await _client.SearchRationsEndpointAsync("1", filter);
        if(result.Items == null)
        {
            return new();
        }
        return (List<RationResponse>)result.Items;
    }

    private async Task<List<GrowthTreatmentResponse>> LoadGrowthTreatmentsAsync()
    {
        var filter = new PaginationFilter { PageSize = 1000 };

        var result = await _client.SearchGrowthTreatmentsEndpointAsync("1", filter);
        if (result.Items == null)
        {
            return new();
        }
        return (List<GrowthTreatmentResponse>)result.Items;
    }

    private async Task<List<PreventativeTreatmentResponse>> LoadPreventativeTreatmentsAsync()
    {
        var filter = new PaginationFilter { PageSize = 1000 };

        var result = await _client.SearchPreventativeTreatmentsEndpointAsync("1", filter);
        if (result.Items == null)
        {
            return new();
        }
        return (List<PreventativeTreatmentResponse>)result.Items;
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

public class LifecycleStageViewModel : UpdateLifecycleStageCommand
{
    public RationResponse Ration { get; set; } = default!;
    public GrowthTreatmentResponse GrowthTreatment { get; set; } = default!;
    public PreventativeTreatmentResponse PreventativeTreatment { get; set; } = default!;
}
