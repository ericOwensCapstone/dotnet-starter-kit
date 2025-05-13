using System.Reflection.Metadata.Ecma335;
using System.Text;
using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Shared.Authorization;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace FSH.Starter.Blazor.Client.Pages.Members;

public partial class MemberAds
{
    [Inject]
    protected IApiClient _client { get; set; } = default!;

    protected EntityServerTableContext<MemberAdResponse, Guid, MemberAdViewModel> Context { get; set; } = default!;

    private EntityTable<MemberAdResponse, Guid, MemberAdViewModel> _table = default!;

    //Start Subentity Lists
    //End Subentity Lists

    //TODO Member Start - adding the tenantId checking for canUpdateEntityFunc, and canDeleteEntityFunc
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    private string CurrentTenantId { get; set; } = default!;
    //TODO Member End

    //TODO Member Start One Per - properties needed for enforcing only 1 member ad per tenant
    private bool _canCreateIt = true;
    //TODO Member End

    protected override async Task OnInitializedAsync()
    {
        //TODO Member Start - adding the tenantId checking for canUpdateEntityFunc, and canDeleteEntityFunc
        var authState = await AuthState;
        CurrentTenantId = authState.User.FindFirst("tenant")?.Value ?? string.Empty;
        //TODO Member End

        Context = new(
            entityName: "MemberAd",
            entityNamePlural: "MemberAds",
            entityResource: FshResources.MemberAds,
            fields: new() {
                new(memberAd => memberAd.Name, "Name", "Name"),
                new(memberAd => memberAd.Description, "Description", "Description"),
            },
            enableAdvancedSearch: true,
            idFunc: memberAd => memberAd.Id!.Value,
            searchFunc: async filter =>
            {
                var memberAdFilter = filter.Adapt<SearchMemberAdsCommand>();
                //Start filter parameters
                //End filter parameters
                var result = await _client.SearchMemberAdsEndpointAsync("1", memberAdFilter);
                //Start Search Func List code
                //End Search Func List code
                return result.Adapt<PaginationResponse<MemberAdResponse>>();
            },
            createFunc: async memberAd =>
            {
                //Start Create Func code
                await _client.CreateMemberAdEndpointAsync("1", memberAd.Adapt<CreateMemberAdCommand>());
                //End Create Func code

                //TODO Member Start
                _canCreateIt = false;
                //TODO Member End
            },
            //Start Edit Func code
            //End Edit Func code
            updateFunc: async (id, memberAd) =>
            {
                //Start Update Func code
                await _client.UpdateMemberAdEndpointAsync("1", id, memberAd.Adapt<UpdateMemberAdCommand>());
                //End Update Func code
            },
            deleteFunc: async id =>
            {
                //Start Delete Func code
                await _client.DeleteMemberAdEndpointAsync("1", id);
                //End Delete Func code
                //TODO Member Start - updating the _canCreateIt property to true after successfully deleting a member ad
                _canCreateIt = true;
                //TODO Member End
            },
            //TODO Member Start - enforcing one per tenant/member
            canCreateEntityFunc: () => {
                return _canCreateIt;
            },
            //TODO Member End

            //TODO Member Start - adding the tenantId checking canUpdateEntityFunc, and canDeleteEntityFunc
            canUpdateEntityFunc: ad => ad.TenantId == CurrentTenantId, // Only allow editing if the tenant owns the MemberAd
            canDeleteEntityFunc: ad => ad.TenantId == CurrentTenantId  // Only allow deletion if the tenant owns the MemberAd
            //TODO Member End
        );

        //TODO Member Start One Per- adding initialization code to check if the tenant already has a member ad
        var memberAdFilter = new SearchMemberAdsCommand();
        memberAdFilter.TenantId = CurrentTenantId;
        var memberAds = await _client.SearchMemberAdsEndpointAsync("1", memberAdFilter);
        if (memberAds.TotalCount != 0)
        {
            _canCreateIt = false;
        }
        //TODO: Member End

        //Start Subentity Loader calls
        //End Subentity Loader calls
    }

    //Start List Related Code//End List Related Code

    //Start Subentity Loaders
    //End Subentity Loaders

    //Start Subentity Advanced Search
    //End Subentity Advanced Search

}

public class MemberAdViewModel : UpdateMemberAdCommand
{
}
