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

    protected override async Task OnInitializedAsync()
    {
        Context = new(
            entityName: "GrowthTreatment",
            entityNamePlural: "GrowthTreatments",
            entityResource: FshResources.GrowthTreatments,
            fields: new() {
                new(growthTreatment => growthTreatment.Name, "Name", "Name"),
                new(growthTreatment => growthTreatment.Description, "Description", "Description"),
                new(growthTreatment => growthTreatment.DollarsPerHead, "DollarsPerHead", "DollarsPerHead"),
            },
            enableAdvancedSearch: true,
            idFunc: growthTreatment => growthTreatment.Id!.Value,
            searchFunc: async filter =>
            {
                var growthTreatmentFilter = filter.Adapt<SearchGrowthTreatmentsCommand>();
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
    }

}

public class GrowthTreatmentViewModel : UpdateGrowthTreatmentCommand
{
}
