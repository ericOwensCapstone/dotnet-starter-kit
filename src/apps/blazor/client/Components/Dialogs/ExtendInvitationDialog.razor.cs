using System.ComponentModel.DataAnnotations;
using FSH.Starter.Blazor.Infrastructure.Api;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FSH.Starter.Blazor.Client.Components.Dialogs;

public partial class ExtendInvitationDialog
{
    [Inject] protected IApiClient ApiClient { get; set; } = default!;
    [Inject] protected ISnackbar Snackbar { get; set; } = default!;

    [CascadingParameter] 
    IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter] 
    public Guid InvitationId { get; set; }
    
    [Parameter] 
    public DateTime CurrentExpiration { get; set; }

    private readonly ExtendInvitationModel _model = new();
    private bool _isProcessing = false;

    protected override void OnInitialized()
    {
        // Set default to 7 days from now or current expiration + 7 days, whichever is later
        var defaultExtension = DateTime.Today.AddDays(7);
        var currentPlusWeek = CurrentExpiration.Date.AddDays(7);
        
        _model.NewExpirationDate = defaultExtension > currentPlusWeek ? defaultExtension : currentPlusWeek;
        _model.NewExpirationTime = CurrentExpiration.TimeOfDay;
    }

    private void Cancel() => MudDialog.Cancel();

    private async Task Submit()
    {
        var combinedDateTime = GetCombinedDateTime();
        
        if (combinedDateTime <= DateTime.UtcNow.AddMinutes(5))
        {
            Snackbar.Add("Expiration must be at least 5 minutes in the future.", Severity.Error);
            return;
        }

        if (combinedDateTime <= CurrentExpiration)
        {
            Snackbar.Add("New expiration must be later than the current expiration.", Severity.Error);
            return;
        }

        _isProcessing = true;
        StateHasChanged();

        try
        {
            var request = new ExtendInvitationExpirationRequest
            {
                InvitationId = InvitationId,
                NewExpirationDate = combinedDateTime
            };

            await ApiClient.ExtendInvitationExpirationAsync(InvitationId, request);
            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Failed to extend invitation: {ex.Message}", Severity.Error);
        }
        finally
        {
            _isProcessing = false;
            StateHasChanged();
        }
    }

    private async Task OnValidSubmit()
    {
        await Submit();
    }

    private bool IsCurrentlyExpired() => DateTime.UtcNow > CurrentExpiration;

    private DateTime GetCombinedDateTime()
    {
        var date = _model.NewExpirationDate ?? DateTime.Today.AddDays(7);
        var time = _model.NewExpirationTime ?? TimeSpan.FromHours(23).Add(TimeSpan.FromMinutes(59));
        return date.Date.Add(time);
    }

    private class ExtendInvitationModel
    {
        [Required(ErrorMessage = "Expiration date is required")]
        public DateTime? NewExpirationDate { get; set; }

        [Required(ErrorMessage = "Expiration time is required")]
        public TimeSpan? NewExpirationTime { get; set; }
    }
}