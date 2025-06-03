using Ardalis.Specification;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Exceptions;
using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Core.Identity.Invitations.Features;
using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Framework.Core.Mail;
using FSH.Framework.Core.Origin;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Tenant.Abstractions;
using FSH.Framework.Infrastructure.Graph.Models;
using FSH.Framework.Infrastructure.Graph.Services;
using FSH.Framework.Infrastructure.Identity.Users;
using FSH.Framework.Infrastructure.Tenant;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;

namespace FSH.Framework.Infrastructure.Identity.Invitations;

public class InvitationService : IInvitationService
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IGraphService _graphService;
    private readonly IMailService _mailService;
    private readonly UserManager<FshUser> _userManager;
    private readonly ITenantService _tenantService;
    private readonly ICurrentUser _currentUser;
    private readonly OriginOptions _originOptions;
    private readonly ILogger<InvitationService> _logger;

    public InvitationService(
        IInvitationRepository invitationRepository,
        IGraphService graphService,
        IMailService mailService,
        UserManager<FshUser> userManager,
        ITenantService tenantService,
        ICurrentUser currentUser,
        IOptions<OriginOptions> originOptions,
        ILogger<InvitationService> logger)
    {
        _invitationRepository = invitationRepository;
        _graphService = graphService;
        _mailService = mailService;
        _userManager = userManager;
        _tenantService = tenantService;
        _currentUser = currentUser;
        _originOptions = originOptions.Value;
        _logger = logger;
    }

    public async Task<CreateInvitationResponse> CreateInvitationAsync(CreateInvitationRequest request, CancellationToken cancellationToken = default)
    {
        // Validate tenant exists
        var tenant = await _tenantService.GetByIdAsync(request.TenantId);
        if (tenant == null)
        {
            throw new NotFoundException($"Tenant {request.TenantId} not found.");
        }

        // Check if user already exists in the system
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new CustomException($"User with email {request.Email} already exists in the system.");
        }

        // Check for existing active invitation
        if (await _invitationRepository.ExistsActiveInvitationAsync(request.Email, request.TenantId, cancellationToken))
        {
            throw new CustomException($"An active invitation already exists for {request.Email} in this tenant.");
        }

        // Check if user exists in B2C
        var b2cUser = await _graphService.GetUserByEmailAsync(request.Email, cancellationToken);
        if (b2cUser != null)
        {
            _logger.LogInformation("User {Email} already exists in B2C with ID {B2CUserId}", request.Email, b2cUser.Id);
        }

        // Create invitation
        var invitation = UserInvitation.Create(
            request.Email,
            request.DisplayName,
            request.TenantId,
            _currentUser.GetUserEmail() ?? "System",
            request.FirstName,
            request.LastName,
            request.Role,
            request.ExpiresAt);

        await _invitationRepository.AddAsync(invitation, cancellationToken);
        await _invitationRepository.SaveChangesAsync(cancellationToken);

        // Create B2C user if doesn't exist
        string? b2cUserId = b2cUser?.Id;
        if (b2cUser == null)
        {
            try
            {
                var temporaryPassword = await _graphService.GenerateTemporaryPassword();
                var graphUser = new GraphUser
                {
                    DisplayName = request.DisplayName,
                    GivenName = request.FirstName,
                    Surname = request.LastName,
                    Mail = request.Email,
                    TenantId = request.TenantId,
                    InvitedBy = _currentUser.GetUserEmail() ?? "System",
                    InvitationDate = DateTime.UtcNow,
                    UserStatus = "Invited"
                };

                var createdUser = await _graphService.CreateUserAsync(graphUser, temporaryPassword, cancellationToken);
                b2cUserId = createdUser.Id;
                
                invitation.MarkAsSent(b2cUserId);
                await _invitationRepository.UpdateAsync(invitation, cancellationToken);
                await _invitationRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create B2C user for {Email}", request.Email);
                invitation.MarkAsFailed($"Failed to create B2C user: {ex.Message}");
                await _invitationRepository.UpdateAsync(invitation, cancellationToken);
                await _invitationRepository.SaveChangesAsync(cancellationToken);
            }
        }
        else
        {
            invitation.MarkAsSent(b2cUserId);
            await _invitationRepository.UpdateAsync(invitation, cancellationToken);
            await _invitationRepository.SaveChangesAsync(cancellationToken);
        }

        // Send invitation email
        bool emailSent = false;
        if (request.SendInvitationEmail && invitation.Status == InvitationStatus.Sent)
        {
            emailSent = await SendInvitationEmailAsync(invitation, tenant.Name, cancellationToken);
        }

        return new CreateInvitationResponse
        {
            InvitationId = invitation.Id,
            Email = invitation.Email,
            DisplayName = invitation.DisplayName,
            TenantId = invitation.TenantId,
            Status = invitation.Status,
            ExpiresAt = invitation.ExpiresAt,
            B2CUserId = b2cUserId,
            EmailSent = emailSent
        };
    }

    public async Task<bool> ResendInvitationAsync(Guid invitationId, CancellationToken cancellationToken = default)
    {
        var invitation = await _invitationRepository.GetByIdAsync(invitationId, cancellationToken);
        if (invitation == null)
        {
            throw new NotFoundException($"Invitation {invitationId} not found.");
        }

        if (invitation.Status != InvitationStatus.Sent)
        {
            throw new CustomException($"Can only resend invitations with status 'Sent'. Current status: {invitation.Status}");
        }

        if (invitation.IsExpired)
        {
            throw new CustomException("Cannot resend an expired invitation.");
        }

        var tenant = await _tenantService.GetByIdAsync(invitation.TenantId);
        if (tenant == null)
        {
            throw new NotFoundException($"Tenant {invitation.TenantId} not found.");
        }

        return await SendInvitationEmailAsync(invitation, tenant.Name, cancellationToken);
    }

    public async Task<bool> CancelInvitationAsync(Guid invitationId, CancellationToken cancellationToken = default)
    {
        var invitation = await _invitationRepository.GetByIdAsync(invitationId, cancellationToken);
        if (invitation == null)
        {
            throw new NotFoundException($"Invitation {invitationId} not found.");
        }

        invitation.Cancel();
        await _invitationRepository.UpdateAsync(invitation, cancellationToken);
        await _invitationRepository.SaveChangesAsync(cancellationToken);

        // Disable the B2C user if exists
        if (!string.IsNullOrEmpty(invitation.B2CUserId))
        {
            try
            {
                await _graphService.DisableUserAsync(invitation.B2CUserId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to disable B2C user {B2CUserId} for cancelled invitation", invitation.B2CUserId);
            }
        }

        return true;
    }

    public async Task<UserInvitation?> GetInvitationAsync(Guid invitationId, CancellationToken cancellationToken = default)
    {
        return await _invitationRepository.GetByIdAsync(invitationId, cancellationToken);
    }

    public async Task<UserInvitation?> GetInvitationByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _invitationRepository.GetByTokenAsync(token, cancellationToken);
    }

    public async Task<List<UserInvitation>> GetPendingInvitationsAsync(string? tenantId = null, CancellationToken cancellationToken = default)
    {
        return await _invitationRepository.GetPendingAsync(tenantId, cancellationToken);
    }

    public async Task<List<UserInvitation>> GetInvitationsByTenantAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        return await _invitationRepository.GetByTenantAsync(tenantId, cancellationToken);
    }

    public async Task<List<UserInvitation>> GetInvitationsByUserAsync(string userEmail, CancellationToken cancellationToken = default)
    {
        return await _invitationRepository.ListAsync(
            new InvitationsByEmailSpec(userEmail), 
            cancellationToken);
    }

    public async Task ProcessPendingInvitationsAsync(CancellationToken cancellationToken = default)
    {
        var pendingInvitations = await _invitationRepository.GetPendingAsync(null, cancellationToken);
        
        foreach (var invitation in pendingInvitations)
        {
            try
            {
                // Create B2C user if not already created
                if (string.IsNullOrEmpty(invitation.B2CUserId))
                {
                    var temporaryPassword = await _graphService.GenerateTemporaryPassword();
                    var graphUser = new GraphUser
                    {
                        DisplayName = invitation.DisplayName,
                        GivenName = invitation.FirstName,
                        Surname = invitation.LastName,
                        Mail = invitation.Email,
                        TenantId = invitation.TenantId,
                        InvitedBy = invitation.InvitedBy,
                        InvitationDate = DateTime.UtcNow,
                        UserStatus = "Invited"
                    };

                    var createdUser = await _graphService.CreateUserAsync(graphUser, temporaryPassword, cancellationToken);
                    invitation.MarkAsSent(createdUser.Id);
                }
                else
                {
                    invitation.MarkAsSent(invitation.B2CUserId);
                }

                await _invitationRepository.UpdateAsync(invitation, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process invitation {InvitationId}", invitation.Id);
                invitation.MarkAsFailed(ex.Message);
                await _invitationRepository.UpdateAsync(invitation, cancellationToken);
            }
        }

        await _invitationRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task ProcessExpiredInvitationsAsync(CancellationToken cancellationToken = default)
    {
        var expiredInvitations = await _invitationRepository.GetExpiredAsync(cancellationToken);
        
        foreach (var invitation in expiredInvitations)
        {
            invitation.MarkAsExpired();
            await _invitationRepository.UpdateAsync(invitation, cancellationToken);

            // Disable the B2C user if exists
            if (!string.IsNullOrEmpty(invitation.B2CUserId))
            {
                try
                {
                    await _graphService.DisableUserAsync(invitation.B2CUserId, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to disable B2C user {B2CUserId} for expired invitation", invitation.B2CUserId);
                }
            }
        }

        await _invitationRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> AcceptInvitationAsync(string token, string userId, CancellationToken cancellationToken = default)
    {
        var invitation = await _invitationRepository.GetByTokenAsync(token, cancellationToken);
        if (invitation == null)
        {
            throw new NotFoundException("Invalid invitation token.");
        }

        if (invitation.IsExpired)
        {
            throw new CustomException("This invitation has expired.");
        }

        if (invitation.Status != InvitationStatus.Sent)
        {
            throw new CustomException($"This invitation cannot be accepted. Status: {invitation.Status}");
        }

        invitation.Accept(userId);
        await _invitationRepository.UpdateAsync(invitation, cancellationToken);
        await _invitationRepository.SaveChangesAsync(cancellationToken);

        // Update B2C user status
        if (!string.IsNullOrEmpty(invitation.B2CUserId))
        {
            try
            {
                await _graphService.UpdateUserCustomAttributesAsync(
                    invitation.B2CUserId,
                    new Dictionary<string, object>
                    {
                        ["UserStatus"] = "Active"
                    },
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update B2C user status for {B2CUserId}", invitation.B2CUserId);
            }
        }

        return true;
    }

    public async Task<bool> IsEmailInvitedAsync(string email, string? tenantId = null, CancellationToken cancellationToken = default)
    {
        ISpecification<UserInvitation> spec;
        if (!string.IsNullOrEmpty(tenantId))
        {
            spec = new InvitationsByEmailAndTenantSpec(email, tenantId);
        }
        else
        {
            spec = new InvitationsByEmailSpec(email);
        }

        return await _invitationRepository.AnyAsync(spec, cancellationToken);
    }

    public async Task<bool> HasPendingInvitationAsync(string email, string tenantId, CancellationToken cancellationToken = default)
    {
        return await _invitationRepository.ExistsActiveInvitationAsync(email, tenantId, cancellationToken);
    }

    private async Task<bool> SendInvitationEmailAsync(UserInvitation invitation, string tenantName, CancellationToken cancellationToken)
    {
        try
        {
            var acceptUrl = $"{_originOptions.OriginUrl}/accept-invitation?token={invitation.InvitationToken}";
            var subject = string.Format(EmailTemplates.UserInvitation.Subject, tenantName);
            var body = EmailTemplates.UserInvitation.GetHtmlBody(
                tenantName,
                invitation.InvitedBy,
                acceptUrl,
                invitation.DisplayName);

            var mailRequest = new MailRequest(
                new Collection<string> { invitation.Email },
                subject,
                body);

            await _mailService.SendAsync(mailRequest, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send invitation email to {Email}", invitation.Email);
            return false;
        }
    }
}