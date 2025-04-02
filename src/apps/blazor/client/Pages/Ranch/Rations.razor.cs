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

    //Start Subentity Lists
    //End Subentity Lists

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
                //Start filter parameters
                //End filter parameters
                var result = await _client.SearchRationsEndpointAsync("1", rationFilter);
                //Start Search Func List code
                //End Search Func List code
                return result.Adapt<PaginationResponse<RationResponse>>();
            },
            createFunc: async ration =>
            {
                //Start Create Func code
                await _client.CreateRationEndpointAsync("1", ration.Adapt<CreateRationCommand>());
                //End Create Func code
            },
            // Start Edit Func code
            // End Edit Func code
            updateFunc: async (id, ration) =>
            {
                //Start Update Func code
                await _client.UpdateRationEndpointAsync("1", id, ration.Adapt<UpdateRationCommand>());
                //End Update Func code
            },
            deleteFunc: async id => await _client.DeleteRationEndpointAsync("1", id));

        //Start Subentity Loader calls
        //End Subentity Loader calls
    }

    //Start List Related Code
    //End List Related Code

    //Start Subentity Loaders
    //End Subentity Loaders

    //Start Subentity Advanced Search
    //End Subentity Advanced Search

}

public class RationViewModel : UpdateRationCommand
{
}
