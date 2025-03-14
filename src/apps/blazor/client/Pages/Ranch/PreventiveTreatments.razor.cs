using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace FSH.Starter.Blazor.Client.Pages.Ranch;

public partial class PreventiveTreatments
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<PreventiveTreatmentResponse, Guid, PreventiveTreatmentViewModel> Context { get; set; } = default!;

    private EntityTable<PreventiveTreatmentResponse, Guid, PreventiveTreatmentViewModel> _table = default!;

    protected override async Task OnInitializedAsync()
    {
        Context = new(
            entityName: "PreventiveTreatment",
            entityNamePlural: "PreventiveTreatments",
            entityResource: FshResources.PreventiveTreatments,
            fields: new() {
                new(preventiveTreatment => preventiveTreatment.Name, "Name", "Name"),
                new(preventiveTreatment => preventiveTreatment.Description, "Description", "Description"),
                new(preventiveTreatment => preventiveTreatment.DollarsPerHead, "DollarsPerHead", "DollarsPerHead"),
            },
            enableAdvancedSearch: true,
            idFunc: preventiveTreatment => preventiveTreatment.Id!.Value,
            searchFunc: async filter =>
            {
                var preventiveTreatmentFilter = filter.Adapt<SearchPreventiveTreatmentsCommand>();
                var result = await _client.SearchPreventiveTreatmentsEndpointAsync("1", preventiveTreatmentFilter);
                return result.Adapt<PaginationResponse<PreventiveTreatmentResponse>>();
            },
            createFunc: async preventiveTreatment =>
            {
                await _client.CreatePreventiveTreatmentEndpointAsync("1", preventiveTreatment.Adapt<CreatePreventiveTreatmentCommand>());
            },
            updateFunc: async (id, preventiveTreatment) =>
            {
                await _client.UpdatePreventiveTreatmentEndpointAsync("1", id, preventiveTreatment.Adapt<UpdatePreventiveTreatmentCommand>());
            },
            deleteFunc: async id => await _client.DeletePreventiveTreatmentEndpointAsync("1", id));
    }

}

public class PreventiveTreatmentViewModel : UpdatePreventiveTreatmentCommand
{
}
