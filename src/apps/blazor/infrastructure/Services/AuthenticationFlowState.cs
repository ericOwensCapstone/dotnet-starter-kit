using System;

namespace FSH.Starter.Blazor.Infrastructure.Services;

public interface IAuthenticationFlowState
{
    bool IsReturningFromAuthentication { get; set; }
    void SetReturningFromAuthentication();
    void ClearReturningFromAuthentication();
}

public class AuthenticationFlowState : IAuthenticationFlowState
{
    private bool _isReturningFromAuthentication;

    public bool IsReturningFromAuthentication 
    { 
        get => _isReturningFromAuthentication;
        set => _isReturningFromAuthentication = value;
    }

    public void SetReturningFromAuthentication()
    {
        _isReturningFromAuthentication = true;
    }

    public void ClearReturningFromAuthentication()
    {
        _isReturningFromAuthentication = false;
    }
}