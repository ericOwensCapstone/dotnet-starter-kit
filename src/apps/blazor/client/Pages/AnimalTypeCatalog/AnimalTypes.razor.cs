using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Shared;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace FSH.Starter.Blazor.Client.Pages.AnimalTypeCatalog;

public partial class AnimalTypes
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<AnimalTypeResponse, Guid, AnimalTypeViewModel> Context { get; set; } = default!;

    private EntityTable<AnimalTypeResponse, Guid, AnimalTypeViewModel> _table = default!;

    protected override void OnInitialized() =>
        Context = new(
            entityName: "AnimalType",
            entityNamePlural: "AnimalTypes",
            entityResource: FshResources.AnimalTypes,
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
                var animalTypeFilter = filter.Adapt<PaginationFilter>();

                var result = await _client.SearchAnimalTypesEndpointAsync("1", animalTypeFilter);
                return result.Adapt<PaginationResponse<AnimalTypeResponse>>();
            },
            createFunc: async prod =>
            {
                await _client.CreateAnimalTypeEndpointAsync("1", prod.Adapt<CreateAnimalTypeCommand>());
            },
            updateFunc: async (id, prod) =>
            {
                await _client.UpdateAnimalTypeEndpointAsync("1", id, prod.Adapt<UpdateAnimalTypeCommand>());
            },
            deleteFunc: async id => await _client.DeleteAnimalTypeEndpointAsync("1", id));

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

public class AnimalTypeViewModel : UpdateAnimalTypeCommand
{
}
