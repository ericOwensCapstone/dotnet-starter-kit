using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace FSH.Starter.Blazor.Infrastructure.Auth;

public class AuthFlowStateService
{
    private readonly IJSRuntime _jsRuntime;
    private const string STATE_KEY = "authFlowState";

    public AuthFlowStateService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SetStateAsync(AuthFlowState state)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", STATE_KEY, state.ToString());
    }

    public async Task<AuthFlowState> GetStateAsync()
    {
        var stateString = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", STATE_KEY);
        
        if (string.IsNullOrEmpty(stateString))
        {
            return AuthFlowState.Initial;
        }

        if (Enum.TryParse<AuthFlowState>(stateString, out var state))
        {
            return state;
        }

        return AuthFlowState.Initial;
    }

    public async Task ClearStateAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", STATE_KEY);
    }
}

public enum AuthFlowState
{
    Initial,
    RedirectingToB2C,
    ProcessingB2CReturn
}