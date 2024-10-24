using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Client.Pages.LifecycleProgramCatalog;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Shared;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace FSH.Starter.Blazor.Client.Pages.WeatherZoneCatalog;

public partial class WeatherZones
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<WeatherZoneResponse, Guid, WeatherZoneViewModel> Context { get; set; } = default!;

    private EntityTable<WeatherZoneResponse, Guid, WeatherZoneViewModel> _table = default!;

    private DateTime? TempPeakTempDate = new DateTime(2024, 8, 1);

    protected override void OnInitialized() =>
        Context = new(
            entityName: "WeatherZone",
            entityNamePlural: "WeatherZones",
            entityResource: FshResources.WeatherZones,
            fields: new()
            {
                //new(prod => prod.Id,"Id", "Id"),
                new(prod => prod.Name,"Name", "Name"),
                new(prod => prod.Description, "Description", "Description"),
                new(prod => prod.YearlyAverageTemp, "Yearly Average Temp", "Yearly Average Temp"),
                new(prod => prod.TempRange, "Temp Range", "Temp Range"),
                new(prod => prod.DeviationPeriod, "Deviation Period", "Deviation Period"),
                new(prod => prod.DeviationAmplitude, "Deviation Amplitude", "Deviation Amplitude"),
                new(prod => DateOnly.FromDateTime(prod.PeakTempDate), "Peak Temp Date", "Peak Temp Date"),
            },
            enableAdvancedSearch: true,
            idFunc: prod => prod.Id!.Value,
            searchFunc: async filter =>
            {
                var weatherZoneFilter = filter.Adapt<PaginationFilter>();

                var result = await _client.SearchWeatherZonesEndpointAsync("1", weatherZoneFilter);
                return result.Adapt<PaginationResponse<WeatherZoneResponse>>();
            },
            editFormInitializedFunc: async () =>
            {
                try
                {
                    await Task.Delay(1);
                    var temp = Context.AddEditModal.RequestModel;
                    if(temp.PeakTempDate.Equals(new DateTime(1,1,1)))
                    {
                        TempPeakTempDate = new DateTime(2024, 8, 1);
                    }
                    else
                    {
                        TempPeakTempDate = temp.PeakTempDate;
                    }
                    Context.AddEditModal.ForceRender();
                }
                catch (Exception ex)
                {
                    var wd = 40;
                }
            },
            createFunc: async prod =>
            {
                var creation = prod.Adapt<CreateWeatherZoneCommand>();
                if (TempPeakTempDate != null)
                {
                    creation.PeakTempDate = (DateTime)TempPeakTempDate;
                }
                else
                {
                    creation.PeakTempDate = new DateTime(2024, 8, 1);
                }

                await _client.CreateWeatherZoneEndpointAsync("1", creation);
            },
            updateFunc: async (id, prod) =>
            {
                await _client.UpdateWeatherZoneEndpointAsync("1", id, prod.Adapt<UpdateWeatherZoneCommand>());
            },
            deleteFunc: async id => await _client.DeleteWeatherZoneEndpointAsync("1", id));

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

public class WeatherZoneViewModel : UpdateWeatherZoneCommand
{
}
