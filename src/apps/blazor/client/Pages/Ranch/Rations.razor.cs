using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace FSH.Starter.Blazor.Client.Pages.Ranch;

public partial class Rations
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<RationResponse, Guid, RationViewModel> Context { get; set; } = default!;

    private EntityTable<RationResponse, Guid, RationViewModel> _table = default!;

    private List<BrandResponse> _brands = new();

    protected override async Task OnInitializedAsync()
    {
        Context = new(
            entityName: "Ration",
            entityNamePlural: "Rations",
            entityResource: FshResources.Rations,
            fields: new()
            {
                new(ration => ration.Id,"Id", "Id"),
                new(ration => ration.Name,"Name", "Name"),
                new(ration => ration.Description, "Description", "Description"),
                new(ration => ration.Price, "Price", "Price"),
            },
            enableAdvancedSearch: true,
            idFunc: ration => ration.Id!.Value,
            searchFunc: async filter =>
            {
                var rationFilter = filter.Adapt<SearchRationsCommand>();
                rationFilter.MinimumRate = Convert.ToDouble(SearchMinimumRate);
                rationFilter.MaximumRate = Convert.ToDouble(SearchMaximumRate);
                var result = await _client.SearchRationsEndpointAsync("1", rationFilter);
                return result.Adapt<PaginationResponse<RationResponse>>();
            },
            createFunc: async ration =>
            {
                await _client.CreateRationEndpointAsync("1", ration.Adapt<CreateRationCommand>());
            },
            updateFunc: async (id, ration) =>
            {
                await _client.UpdateRationEndpointAsync("1", id, ration.Adapt<UpdateRationCommand>());
            },
            deleteFunc: async id => await _client.DeleteRationEndpointAsync("1", id));

        await LoadBrandsAsync();
    }

    private async Task LoadBrandsAsync()
    {
        if (_brands.Count == 0)
        {
            var response = await _client.SearchBrandsEndpointAsync("1", new SearchBrandsCommand());
            if (response?.Items != null)
            {
                _brands = response.Items.ToList();
            }
        }
    }

    // Advanced Search

    private Guid? _searchBrandId;
    private Guid? SearchBrandId
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

public class RationViewModel : UpdateRationCommand
{
}
