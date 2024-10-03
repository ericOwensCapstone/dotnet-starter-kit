using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Shared;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace FSH.Starter.Blazor.Client.Pages.GrowthTreatmentCatalog;

public partial class GrowthTreatments
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<GrowthTreatmentResponse, Guid, GrowthTreatmentViewModel> Context { get; set; } = default!;

    private EntityTable<GrowthTreatmentResponse, Guid, GrowthTreatmentViewModel> _table = default!;

    protected override void OnInitialized() =>
    
        Context = new(
            entityName: "GrowthTreatment",
            entityNamePlural: "GrowthTreatments",
            entityResource: FshResources.GrowthTreatments,
            fields: new()
            {
                new(prod => prod.Id,"Id", "Id"),
                new(prod => prod.Name,"Name", "Name"),
                new(prod => prod.Description, "Description", "Description"),
                new(prod => prod.DollarsPerHead, "DollarsPerHead", "DollarsPerHead")
            },
            enableAdvancedSearch: true,
            idFunc: prod => prod.Id!.Value,
            searchFunc: async filter =>
            {
                var growthTreatmentFilter = filter.Adapt<PaginationFilter>();

                var result = await _client.SearchGrowthTreatmentsEndpointAsync("1", growthTreatmentFilter);
                return result.Adapt<PaginationResponse<GrowthTreatmentResponse>>();
            },
            createFunc: async prod =>
            {
                await _client.CreateGrowthTreatmentEndpointAsync("1", prod.Adapt<CreateGrowthTreatmentCommand>());
            },
            updateFunc: async (id, prod) =>
            {
                await _client.UpdateGrowthTreatmentEndpointAsync("1", id, prod.Adapt<UpdateGrowthTreatmentCommand>());
            },
            deleteFunc: async id => await _client.DeleteGrowthTreatmentEndpointAsync("1", id));
    

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

public class GrowthTreatmentViewModel : UpdateGrowthTreatmentCommand
{
}
