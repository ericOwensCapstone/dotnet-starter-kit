using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Shared;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace FSH.Starter.Blazor.Client.Pages.PreventativeTreatmentCatalog;

public partial class PreventativeTreatments
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<PreventativeTreatmentResponse, Guid, PreventativeTreatmentViewModel> Context { get; set; } = default!;

    private EntityTable<PreventativeTreatmentResponse, Guid, PreventativeTreatmentViewModel> _table = default!;

    protected override void OnInitialized() =>
    
        Context = new(
            entityName: "PreventativeTreatment",
            entityNamePlural: "PreventativeTreatments",
            entityResource: FshResources.PreventativeTreatments,
            fields: new()
            {
                //new(prod => prod.Id,"Id", "Id"),
                new(prod => prod.Name,"Name", "Name"),
                new(prod => prod.Description, "Description", "Description"),
                new(prod => prod.DollarsPerHead, "DollarsPerHead", "DollarsPerHead")
            },
            enableAdvancedSearch: true,
            idFunc: prod => prod.Id!.Value,
            searchFunc: async filter =>
            {
                var preventativeTreatmentFilter = filter.Adapt<PaginationFilter>();

                var result = await _client.SearchPreventativeTreatmentsEndpointAsync("1", preventativeTreatmentFilter);
                return result.Adapt<PaginationResponse<PreventativeTreatmentResponse>>();
            },
            createFunc: async prod =>
            {
                await _client.CreatePreventativeTreatmentEndpointAsync("1", prod.Adapt<CreatePreventativeTreatmentCommand>());
            },
            updateFunc: async (id, prod) =>
            {
                await _client.UpdatePreventativeTreatmentEndpointAsync("1", id, prod.Adapt<UpdatePreventativeTreatmentCommand>());
            },
            deleteFunc: async id => await _client.DeletePreventativeTreatmentEndpointAsync("1", id));
    

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

public class PreventativeTreatmentViewModel : UpdatePreventativeTreatmentCommand
{
}
