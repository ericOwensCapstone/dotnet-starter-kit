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
                new(prod => prod.FcrMean, "Fcr Mean", "Fcr Mean"),
                new(prod => prod.FcrStdDev, "Fcr StdDev", "Fcr StdDev"),
                new(prod => prod.DiseaseIncidenceMean, "Disease Incidence Mean", "Disease Incidence Mean"),
                new(prod => prod.DiseaseIncidenceStdDev, "Disease Incidence StdDev", "Disease Incidence StdDev"),
                new(prod => prod.CarcassYieldMean, "Carcass Yield Mean", "Carcass Yield Mean"),
                new(prod => prod.CarcassYieldStdDev, "Carcass Yield StdDev", "Carcass Yield StdDev"),
                new(prod => prod.QualityGradeMean, "Quality Grade Mean", "Quality Grade Mean"),
                new(prod => prod.QualityGradeStdDev, "Quality Grade StdDev", "Quality Grade StdDev"),
                new(prod => prod.LowerCriticalTemp, "Lower Critical Temp", "Lower Critical Temp"),
                new(prod => prod.UpperCriticalTemp, "Upper Critical Temp", "Upper Critical Temp"),
                new(prod => prod.ArrivalHeadCountMean, "Arrival Head Count Mean", "Arrival Head Count Mean"),
                new(prod => prod.ArrivalHeadCountStdDev, "Arrival Head Count StdDev", "Arrival Head Count StdDev"),
                new(prod => prod.ArrivalWeightMean, "Arrival Weight Mean", "Arrival Weight Mean"),
                new(prod => prod.ArrivalWeightStdDev, "Arrival Weight StdDev", "Arrival Weight StdDev"),
                new(prod => prod.ArrivalCostPerCwtMean, "Arrival Cost Mean", "Arrival Cost Mean"),
                new(prod => prod.ArrivalCostPerCwtStdDev, "Arrival Cost StdDev", "Arrival Cost StdDev"),
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
