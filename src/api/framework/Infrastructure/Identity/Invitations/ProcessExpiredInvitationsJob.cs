using FSH.Framework.Core.Identity.Invitations;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace FSH.Framework.Infrastructure.Identity.Invitations;

public class ProcessExpiredInvitationsJob
{
    private readonly IInvitationService _invitationService;
    private readonly ILogger<ProcessExpiredInvitationsJob> _logger;

    public ProcessExpiredInvitationsJob(
        IInvitationService invitationService,
        ILogger<ProcessExpiredInvitationsJob> logger)
    {
        _invitationService = invitationService;
        _logger = logger;
    }

    [Queue("default")]
    [AutomaticRetry(Attempts = 3)]
    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing expired invitations...");
        
        try
        {
            await _invitationService.ProcessExpiredInvitationsAsync(cancellationToken);
            _logger.LogInformation("Expired invitations processed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing expired invitations");
            throw;
        }
    }
}