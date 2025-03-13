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

    protected override async Task OnInitializedAsync()
    {
        Context = new(
            entityName: "Ration",
            entityNamePlural: "Rations",
            entityResource: FshResources.Rations,
            fields: new()
            {
                new(ration => ration.Name,"Name", "Name"),
                new(ration => ration.Description, "Description", "Description"),
                new(ration => ration.DollarsPerPound, "DollarsPerPound", "DollarsPerPound"),
            },
            enableAdvancedSearch: true,
            idFunc: ration => ration.Id!.Value,
            searchFunc: async filter =>
            {
                var rationFilter = filter.Adapt<SearchRationsCommand>();
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
    }

}

public class RationViewModel : UpdateRationCommand
{
}
