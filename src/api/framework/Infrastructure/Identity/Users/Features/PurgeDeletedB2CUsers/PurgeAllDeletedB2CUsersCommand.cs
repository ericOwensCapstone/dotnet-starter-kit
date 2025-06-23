using MediatR;

namespace FSH.Framework.Infrastructure.Identity.Users.Features.PurgeDeletedB2CUsers;

public class PurgeAllDeletedB2CUsersCommand : IRequest<PurgeAllDeletedB2CUsersResponse>
{
    // No parameters needed
}

public class PurgeAllDeletedB2CUsersResponse
{
    public bool Success { get; set; }
    public int TotalCount { get; set; }
    public List<PurgeResult> Results { get; set; } = new();
}

public class PurgeResult
{
    public bool Success { get; set; }
    public string Email { get; set; } = default!;
    public string? Error { get; set; }
}