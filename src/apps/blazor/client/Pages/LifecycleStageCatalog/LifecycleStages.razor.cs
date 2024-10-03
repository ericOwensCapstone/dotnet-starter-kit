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

    protected override void OnInitialized() =>
    
        Context = new(
            entityName: "LifecycleStage",
            entityNamePlural: "LifecycleStages",
            entityResource: FshResources.LifecycleStages,
            fields: new()
            {
                new(prod => prod.Id,"Id", "Id"),
                new(prod => prod.Name,"Name", "Name"),
                new(prod => prod.Description, "Description", "Description"),
                new(prod => prod.Rating, "Rating", "Rating")
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
}
