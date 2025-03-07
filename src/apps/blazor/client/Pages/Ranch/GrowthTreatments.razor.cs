using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace FSH.Starter.Blazor.Client.Pages.Ranch;

public partial class GrowthTreatments
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<GrowthTreatmentResponse, Guid, GrowthTreatmentViewModel> Context { get; set; } = default!;

    private EntityTable<GrowthTreatmentResponse, Guid, GrowthTreatmentViewModel> _table = default!;

    //private List<BrandResponse> _brands = new();

    protected override async Task OnInitializedAsync()
    {
        Context = new(
            entityName: "GrowthTreatment",
            entityNamePlural: "GrowthTreatments",
            entityResource: FshResources.GrowthTreatments,
            fields: new()
            {
                new(growthTreatment => growthTreatment.Id,"Id", "Id"),
                new(growthTreatment => growthTreatment.Name,"Name", "Name"),
                new(growthTreatment => growthTreatment.Description, "Description", "Description"),
                new(growthTreatment => growthTreatment.Price, "Price", "Price"),
            },
            enableAdvancedSearch: true,
            idFunc: growthTreatment => growthTreatment.Id!.Value,
            searchFunc: async filter =>
            {
                var growthTreatmentFilter = filter.Adapt<SearchGrowthTreatmentsCommand>();
                growthTreatmentFilter.MinimumRate = Convert.ToDouble(SearchMinimumRate);
                growthTreatmentFilter.MaximumRate = Convert.ToDouble(SearchMaximumRate);
                var result = await _client.SearchGrowthTreatmentsEndpointAsync("1", growthTreatmentFilter);
                return result.Adapt<PaginationResponse<GrowthTreatmentResponse>>();
            },
            createFunc: async growthTreatment =>
            {
                await _client.CreateGrowthTreatmentEndpointAsync("1", growthTreatment.Adapt<CreateGrowthTreatmentCommand>());
            },
            updateFunc: async (id, growthTreatment) =>
            {
                await _client.UpdateGrowthTreatmentEndpointAsync("1", id, growthTreatment.Adapt<UpdateGrowthTreatmentCommand>());
            },
            deleteFunc: async id => await _client.DeleteGrowthTreatmentEndpointAsync("1", id));

        //await LoadBrandsAsync();
    }

    //private async Task LoadBrandsAsync()
    //{
    //    if (_brands.Count == 0)
    //    {
    //        var response = await _client.SearchBrandsEndpointAsync("1", new SearchBrandsCommand());
    //        if (response?.Items != null)
    //        {
    //            _brands = response.Items.ToList();
    //        }
    //    }
    //}

    // Advanced Search

    //private Guid? _searchBrandId;
    //private Guid? SearchBrandId
    //{
    //    get => _searchBrandId;
    //    set
    //    {
    //        _searchBrandId = value;
    //        _ = _table.ReloadDataAsync();
    //    }
    //}

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
