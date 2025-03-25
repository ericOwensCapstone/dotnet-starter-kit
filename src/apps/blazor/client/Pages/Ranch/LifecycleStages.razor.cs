using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace FSH.Starter.Blazor.Client.Pages.Ranch;

public partial class LifecycleStages
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<LifecycleStageResponse, Guid, LifecycleStageViewModel> Context { get; set; } = default!;

    private EntityTable<LifecycleStageResponse, Guid, LifecycleStageViewModel> _table = default!;

    //Start Subentity Lists    
    private List<RationResponse> _rations = new();    
    private List<GrowthTreatmentResponse> _growthTreatments = new();    
    private List<PreventiveTreatmentResponse> _preventiveTreatments = new();
    //End Subentity Lists

    protected override async Task OnInitializedAsync()
    {
        Context = new(
            entityName: "LifecycleStage",
            entityNamePlural: "LifecycleStages",
            entityResource: FshResources.LifecycleStages,
            fields: new() {
                new(lifecycleStage => lifecycleStage.Name, "Name", "Name"),
                new(lifecycleStage => lifecycleStage.Description, "Description", "Description"),
                new(lifecycleStage => lifecycleStage.Ration.Name, "Ration", "Ration"),
                new(lifecycleStage => lifecycleStage.GrowthTreatment.Name, "GrowthTreatment", "GrowthTreatment"),
                new(lifecycleStage => lifecycleStage.PreventiveTreatment.Name, "PreventiveTreatment", "PreventiveTreatment"),
            },
            enableAdvancedSearch: true,
            idFunc: lifecycleStage => lifecycleStage.Id!.Value,
            searchFunc: async filter =>
            {
                var lifecycleStageFilter = filter.Adapt<SearchLifecycleStagesCommand>();
                //Start filter parameters
                lifecycleStageFilter.RationId = SearchRationId;
                lifecycleStageFilter.GrowthTreatmentId = SearchGrowthTreatmentId;
                lifecycleStageFilter.PreventiveTreatmentId = SearchPreventiveTreatmentId;
                //End filter parameters
                var result = await _client.SearchLifecycleStagesEndpointAsync("1", lifecycleStageFilter);
                return result.Adapt<PaginationResponse<LifecycleStageResponse>>();
            },
            createFunc: async lifecycleStage =>
            {
                await _client.CreateLifecycleStageEndpointAsync("1", lifecycleStage.Adapt<CreateLifecycleStageCommand>());
            },
            updateFunc: async (id, lifecycleStage) =>
            {
                await _client.UpdateLifecycleStageEndpointAsync("1", id, lifecycleStage.Adapt<UpdateLifecycleStageCommand>());
            },
            deleteFunc: async id => await _client.DeleteLifecycleStageEndpointAsync("1", id));

        //Start Subentity Loader calls
        await LoadRationsAsync();
        await LoadGrowthTreatmentsAsync();
        await LoadPreventiveTreatmentsAsync();
        //End Subentity Loader calls
    }

    //Start Subentity Loaders
    private async Task LoadRationsAsync()
    {
        if (_rations.Count == 0)
        {
            var response = await _client.SearchRationsEndpointAsync("1", new SearchRationsCommand());
            if (response?.Items != null)
            {
                _rations = response.Items.ToList();
            }
        }
    }

    private async Task LoadGrowthTreatmentsAsync()
    {
        if (_growthTreatments.Count == 0)
        {
            var response = await _client.SearchGrowthTreatmentsEndpointAsync("1", new SearchGrowthTreatmentsCommand());
            if (response?.Items != null)
            {
                _growthTreatments = response.Items.ToList();
            }
        }
    }

    private async Task LoadPreventiveTreatmentsAsync()
    {
        if (_preventiveTreatments.Count == 0)
        {
            var response = await _client.SearchPreventiveTreatmentsEndpointAsync("1", new SearchPreventiveTreatmentsCommand());
            if (response?.Items != null)
            {
                _preventiveTreatments = response.Items.ToList();
            }
        }
    }

    //End Subentity Loaders

    //Start Subentity Advanced Search

    private Guid? _searchRationId;
    private Guid? SearchRationId
    {
        get => _searchRationId;
        set
        {
            _searchRationId = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private Guid? _searchGrowthTreatmentId;
    private Guid? SearchGrowthTreatmentId
    {
        get => _searchGrowthTreatmentId;
        set
        {
            _searchGrowthTreatmentId = value;
            _ = _table.ReloadDataAsync();
        }
    }

    private Guid? _searchPreventiveTreatmentId;
    private Guid? SearchPreventiveTreatmentId
    {
        get => _searchPreventiveTreatmentId;
        set
        {
            _searchPreventiveTreatmentId = value;
            _ = _table.ReloadDataAsync();
        }
    }
    //End Subentity Advanced Search

}

public class LifecycleStageViewModel : UpdateLifecycleStageCommand
{
}
