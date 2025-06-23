using FSH.Framework.Core.Exceptions;
using FSH.Framework.Infrastructure.Graph.Services;
using FSH.Framework.Infrastructure.Identity.Users;
using FSH.Framework.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FSH.Framework.Infrastructure.Identity.Users.Features.DeleteUserCompletely;

public class DeleteUserCompletelyCommandHandler : IRequestHandler<DeleteUserCompletelyCommand, DeleteUserCompletelyResponse>
{
    private readonly TenantFreeDbContext _dbContext;
    private readonly IGraphService _graphService;
    private readonly ILogger<DeleteUserCompletelyCommandHandler> _logger;

    public DeleteUserCompletelyCommandHandler(
        TenantFreeDbContext dbContext,
        IGraphService graphService,
        ILogger<DeleteUserCompletelyCommandHandler> logger)
    {
        _dbContext = dbContext;
        _graphService = graphService;
        _logger = logger;
    }

    public async Task<DeleteUserCompletelyResponse> Handle(DeleteUserCompletelyCommand request, CancellationToken cancellationToken)
    {
        var response = new DeleteUserCompletelyResponse
        {
            UserId = request.UserId,
            Details = new List<DeleteStepResult>()
        };

        // Get user details first
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException($"User with ID {request.UserId} not found.");
        }

        var userEmail = user.Email;
        var objectId = user.ObjectId;

        // Begin transaction
        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Step 1: Delete from UserRoles
            var userRoleCount = await _dbContext.UserRoles
                .Where(ur => ur.UserId == request.UserId)
                .ExecuteDeleteAsync(cancellationToken);

            response.Details.Add(new DeleteStepResult
            {
                Step = "Delete UserRoles",
                Success = true,
                Message = $"Deleted {userRoleCount} role assignments"
            });

            // Step 2: Delete from Users table
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            response.Details.Add(new DeleteStepResult
            {
                Step = "Delete User Record",
                Success = true,
                Message = "User deleted from database"
            });

            // Step 3: Delete from UserInvitations by email
            var invitationCount = await _dbContext.UserInvitations
                .Where(ui => ui.Email == userEmail)
                .ExecuteDeleteAsync(cancellationToken);

            response.Details.Add(new DeleteStepResult
            {
                Step = "Delete User Invitations",
                Success = true,
                Message = $"Deleted {invitationCount} invitations"
            });

            // Commit transaction
            await transaction.CommitAsync(cancellationToken);

            response.Details.Add(new DeleteStepResult
            {
                Step = "Database Transaction",
                Success = true,
                Message = "All database operations completed successfully"
            });

            // Step 4: Delete from B2C if requested and user has ObjectId
            if (request.DeleteFromB2C && !string.IsNullOrEmpty(objectId))
            {
                try
                {
                    // First soft delete
                    var deleteSuccess = await _graphService.DeleteUserAsync(objectId, cancellationToken);
                    if (deleteSuccess)
                    {
                        response.Details.Add(new DeleteStepResult
                        {
                            Step = "B2C Soft Delete",
                            Success = true,
                            Message = "User soft-deleted from Azure B2C"
                        });

                        // Then permanent delete
                        var permanentDeleteSuccess = await _graphService.PermanentlyDeleteUserAsync(objectId, cancellationToken);
                        if (permanentDeleteSuccess)
                        {
                            response.Details.Add(new DeleteStepResult
                            {
                                Step = "B2C Permanent Delete",
                                Success = true,
                                Message = "User permanently deleted from Azure B2C"
                            });
                        }
                        else
                        {
                            response.Details.Add(new DeleteStepResult
                            {
                                Step = "B2C Permanent Delete",
                                Success = false,
                                Message = "Failed to permanently delete user from B2C (may need to wait for soft delete to process)"
                            });
                        }
                    }
                    else
                    {
                        response.Details.Add(new DeleteStepResult
                        {
                            Step = "B2C Soft Delete",
                            Success = false,
                            Message = "Failed to delete user from Azure B2C"
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting user from B2C: {ObjectId}", objectId);
                    response.Details.Add(new DeleteStepResult
                    {
                        Step = "B2C Delete",
                        Success = false,
                        Message = $"B2C deletion failed: {ex.Message}"
                    });
                }
            }

            // Determine overall success
            response.Success = response.Details.All(d => d.Success || d.Step.Contains("B2C"));
            response.Message = response.Success 
                ? $"User {userEmail} deleted successfully" 
                : $"User {userEmail} deleted from database but some operations failed";

            return response;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            
            _logger.LogError(ex, "Error deleting user {UserId}", request.UserId);
            
            response.Success = false;
            response.Message = $"Failed to delete user: {ex.Message}";
            response.Details.Add(new DeleteStepResult
            {
                Step = "Database Transaction",
                Success = false,
                Message = $"Transaction rolled back: {ex.Message}"
            });
            
            return response;
        }
    }
}