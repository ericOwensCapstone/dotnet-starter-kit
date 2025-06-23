using FSH.Framework.Infrastructure.Graph.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Framework.Infrastructure.Identity.Users.Features.PurgeDeletedB2CUsers;

public class PurgeAllDeletedB2CUsersCommandHandler : IRequestHandler<PurgeAllDeletedB2CUsersCommand, PurgeAllDeletedB2CUsersResponse>
{
    private readonly IGraphService _graphService;
    private readonly ILogger<PurgeAllDeletedB2CUsersCommandHandler> _logger;

    public PurgeAllDeletedB2CUsersCommandHandler(
        IGraphService graphService,
        ILogger<PurgeAllDeletedB2CUsersCommandHandler> logger)
    {
        _graphService = graphService;
        _logger = logger;
    }

    public async Task<PurgeAllDeletedB2CUsersResponse> Handle(PurgeAllDeletedB2CUsersCommand request, CancellationToken cancellationToken)
    {
        var response = new PurgeAllDeletedB2CUsersResponse();

        try
        {
            // Get all deleted users from B2C
            var deletedUsers = await _graphService.GetDeletedUsersAsync(cancellationToken);
            response.TotalCount = deletedUsers.Count;

            _logger.LogInformation("Found {Count} deleted users in B2C to purge", deletedUsers.Count);

            // Process each deleted user
            foreach (var user in deletedUsers)
            {
                try
                {
                    var permanentlyDeleted = await _graphService.PermanentlyDeleteUserAsync(user.Id, cancellationToken);
                    
                    response.Results.Add(new PurgeResult
                    {
                        Success = permanentlyDeleted,
                        Email = user.Mail ?? user.UserPrincipalName ?? user.Id,
                        Error = permanentlyDeleted ? null : "Failed to permanently delete user"
                    });

                    if (permanentlyDeleted)
                    {
                        _logger.LogInformation("Successfully purged user: {Email}", user.Mail);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to purge user: {Email}", user.Mail);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error purging user {UserId} ({Email})", user.Id, user.Mail);
                    
                    response.Results.Add(new PurgeResult
                    {
                        Success = false,
                        Email = user.Mail ?? user.UserPrincipalName ?? user.Id,
                        Error = ex.Message
                    });
                }
            }

            // Determine overall success
            var successCount = response.Results.Count(r => r.Success);
            response.Success = successCount == response.TotalCount;

            _logger.LogInformation("Purge operation completed. Success: {SuccessCount}/{TotalCount}", 
                successCount, response.TotalCount);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving deleted users from B2C");
            
            response.Success = false;
            response.Results.Add(new PurgeResult
            {
                Success = false,
                Email = "N/A",
                Error = $"Failed to retrieve deleted users: {ex.Message}"
            });
            
            return response;
        }
    }
}