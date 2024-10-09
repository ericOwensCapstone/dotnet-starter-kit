using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Shared;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace FSH.Starter.Blazor.Client.Pages.RationCatalog;

public partial class Rations
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<RationResponse, Guid, RationViewModel> Context { get; set; } = default!;

    private EntityTable<RationResponse, Guid, RationViewModel> _table = default!;

    protected override void OnInitialized() =>
        Context = new(
            entityName: "Ration",
            entityNamePlural: "Rations",
            entityResource: FshResources.Rations,
            fields: new()
            {
                //new(prod => prod.Id,"Id", "Id"),
                new(prod => prod.Name,"Name", "Name"),
                new(prod => prod.Description, "Description", "Description"),
                new(prod => prod.DollarsPerPound, "DollarsPerPound", "DollarsPerPound")
            },
            enableAdvancedSearch: true,
            idFunc: prod => prod.Id!.Value,
            searchFunc: async filter =>
            {
                var rationFilter = filter.Adapt<PaginationFilter>();

                var result = await _client.SearchRationsEndpointAsync("1", rationFilter);
                return result.Adapt<PaginationResponse<RationResponse>>();
            },
            createFunc: async prod =>
            {
                await _client.CreateRationEndpointAsync("1", prod.Adapt<CreateRationCommand>());
            },
            updateFunc: async (id, prod) =>
            {
                await _client.UpdateRationEndpointAsync("1", id, prod.Adapt<UpdateRationCommand>());
            },
            deleteFunc: async id => await _client.DeleteRationEndpointAsync("1", id));

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

public class RationViewModel : UpdateRationCommand
{
}
