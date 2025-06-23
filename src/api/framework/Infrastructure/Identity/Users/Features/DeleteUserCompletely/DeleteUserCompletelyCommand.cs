using MediatR;

namespace FSH.Framework.Infrastructure.Identity.Users.Features.DeleteUserCompletely;

public class DeleteUserCompletelyCommand : IRequest<DeleteUserCompletelyResponse>
{
    public string UserId { get; set; } = default!;
    public bool DeleteFromB2C { get; set; }
}

public class DeleteUserCompletelyResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = default!;
    public string? UserId { get; set; }
    public List<DeleteStepResult>? Details { get; set; }
}

public class DeleteStepResult
{
    public string Step { get; set; } = default!;
    public bool Success { get; set; }
    public string Message { get; set; } = default!;
}