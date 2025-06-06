using Ardalis.Specification;
using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Exceptions;
using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Core.Identity.Invitations.Features;
using FSH.Framework.Core.Identity.Invitations.Features.ExtendInvitationExpiration;
using FSH.Framework.Core.Identity.Invitations.Features.SearchInvitations;
using FSH.Framework.Core.Identity.Invitations.Specifications;
using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Framework.Core.Mail;
using FSH.Framework.Core.Origin;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Tenant.Abstractions;
using FSH.Framework.Infrastructure.Graph.Models;
using FSH.Framework.Infrastructure.Graph.Services;
using FSH.Framework.Infrastructure.Identity.Users;
using FSH.Framework.Infrastructure.Tenant;
using Mapster;
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
    private readonly IMultiTenantContextAccessor<FshTenantInfo> _tenantContextAccessor;
    private readonly OriginOptions _originOptions;
    private readonly ILogger<InvitationService> _logger;

    public InvitationService(
        IInvitationRepository invitationRepository,
        IGraphService graphService,
        IMailService mailService,
        UserManager<FshUser> userManager,
        ITenantService tenantService,
        ICurrentUser currentUser,
        IMultiTenantContextAccessor<FshTenantInfo> tenantContextAccessor,
        IOptions<OriginOptions> originOptions,
        ILogger<InvitationService> logger)
    {
        _invitationRepository = invitationRepository;
        _graphService = graphService;
        _mailService = mailService;
        _userManager = userManager;
        _tenantService = tenantService;
        _currentUser = currentUser;
        _tenantContextAccessor = tenantContextAccessor;
        _originOptions = originOptions.Value;
        _logger = logger;
    }

    public async Task<CreateInvitationResponse> CreateInvitationAsync(CreateInvitationRequest request, CancellationToken cancellationToken = default)
    {
        // Validate target tenant exists
        var targetTenant = await _tenantService.GetByIdAsync(request.TargetTenantId);
        if (targetTenant == null)
        {
            throw new NotFoundException($"Target tenant {request.TargetTenantId} not found.");
        }

        // Check if user already exists in the system
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new CustomException($"User with email {request.Email} already exists in the system.");
        }

        // Check for existing active invitation
        if (await _invitationRepository.ExistsActiveInvitationAsync(request.Email, request.TargetTenantId, cancellationToken))
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
            request.TargetTenantId,
            _currentUser.GetUserEmail() ?? "System",
            request.FirstName,
            request.LastName,
            request.Role,
            request.ExpiresAt);

        // Manually set ownership properties (since IdentityDbContext doesn't auto-set ITenantEntity)
        invitation.TenantId = _currentUser.GetTenant();
        invitation.MemberId = _tenantContextAccessor.MultiTenantContext?.TenantInfo?.MemberId;

        await _invitationRepository.AddAsync(invitation, cancellationToken);
        await _invitationRepository.SaveChangesAsync(cancellationToken);

        // Create B2C invitation if user doesn't exist
        string? b2cUserId = b2cUser?.Id;
        if (b2cUser == null && request.SendInvitationEmail)
        {
            try
            {
                // Create B2C invitation which will send the email
                var acceptUrl = $"{_originOptions.OriginUrl}/authentication/login-callback?invitation={invitation.InvitationToken}";
                var graphInvitation = new GraphInvitation
                {
                    Email = request.Email,
                    DisplayName = request.DisplayName,
                    TenantId = request.TargetTenantId,
                    InvitedBy = _currentUser.GetUserEmail() ?? "System",
                    RedirectUrl = acceptUrl,
                    SendInvitationMessage = true,
                    CustomizedMessageBody = $"You have been invited to join {targetTenant.Name}. Click the link below to accept the invitation and create your account."
                };

                b2cUserId = await _graphService.CreateInvitationAsync(graphInvitation, cancellationToken);
                
                invitation.MarkAsSent(b2cUserId);
                await _invitationRepository.UpdateAsync(invitation, cancellationToken);
                await _invitationRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create B2C invitation for {Email}", request.Email);
                invitation.MarkAsFailed($"Failed to create B2C invitation: {ex.Message}");
                await _invitationRepository.UpdateAsync(invitation, cancellationToken);
                await _invitationRepository.SaveChangesAsync(cancellationToken);
            }
        }
        else if (b2cUser != null)
        {
            // User already exists in B2C
            invitation.MarkAsSent(b2cUserId);
            await _invitationRepository.UpdateAsync(invitation, cancellationToken);
            await _invitationRepository.SaveChangesAsync(cancellationToken);
        }
        else
        {
            // Not sending email but user doesn't exist - create disabled user as before
            try
            {
                var temporaryPassword = await _graphService.GenerateTemporaryPassword();
                var graphUser = new GraphUser
                {
                    DisplayName = request.DisplayName,
                    GivenName = request.FirstName,
                    Surname = request.LastName,
                    Mail = request.Email,
                    TenantId = request.TargetTenantId,
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

        // Email is sent by B2C if we created an invitation
        bool emailSent = request.SendInvitationEmail && b2cUser == null && invitation.Status == InvitationStatus.Sent;

        return new CreateInvitationResponse
        {
            InvitationId = invitation.Id,
            Email = invitation.Email,
            DisplayName = invitation.DisplayName,
            TargetTenantId = invitation.TargetTenantId,
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

        var targetTenant = await _tenantService.GetByIdAsync(invitation.TargetTenantId);
        if (targetTenant == null)
        {
            throw new NotFoundException($"Target tenant {invitation.TargetTenantId} not found.");
        }

        return await SendInvitationEmailAsync(invitation, targetTenant.Name, cancellationToken);
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

    public async Task<ExtendInvitationExpirationResponse> ExtendInvitationExpirationAsync(ExtendInvitationExpirationRequest request, CancellationToken cancellationToken = default)
    {
        var invitation = await _invitationRepository.GetByIdAsync(request.InvitationId, cancellationToken);
        if (invitation == null)
        {
            throw new NotFoundException($"Invitation {request.InvitationId} not found.");
        }

        var previousExpiration = invitation.ExpiresAt;

        try
        {
            invitation.ExtendExpiration(request.NewExpirationDate);
            await _invitationRepository.UpdateAsync(invitation, cancellationToken);
            await _invitationRepository.SaveChangesAsync(cancellationToken);

            // If invitation was expired and had a B2C user, re-enable them
            if (invitation.Status == InvitationStatus.Sent && !string.IsNullOrEmpty(invitation.B2CUserId))
            {
                try
                {
                    await _graphService.EnableUserAsync(invitation.B2CUserId, cancellationToken);
                    _logger.LogInformation("Re-enabled B2C user {B2CUserId} for extended invitation {InvitationId}", 
                        invitation.B2CUserId, request.InvitationId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to re-enable B2C user {B2CUserId} for extended invitation {InvitationId}", 
                        invitation.B2CUserId, request.InvitationId);
                }
            }

            return new ExtendInvitationExpirationResponse
            {
                InvitationId = request.InvitationId,
                NewExpirationDate = request.NewExpirationDate,
                PreviousExpirationDate = previousExpiration,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to extend invitation {InvitationId} expiration", request.InvitationId);
            throw;
        }
    }

    public async Task<UserInvitation?> GetInvitationAsync(Guid invitationId, CancellationToken cancellationToken = default)
    {
        return await _invitationRepository.GetByIdAsync(invitationId, cancellationToken);
    }

    public async Task<UserInvitation?> GetInvitationByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _invitationRepository.GetByTokenAsync(token, cancellationToken);
    }

    public async Task<PagedList<InvitationDto>> SearchInvitationsAsync(SearchInvitationsQuery request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SearchInvitationsAsync called with: TenantId={TenantId}, TargetTenantId={TargetTenantId}, Status={Status}, PageNumber={PageNumber}, PageSize={PageSize}", 
            request.TenantId, request.TargetTenantId, request.Status, request.PageNumber, request.PageSize);
        
        // Debug: Log all invitations in database first
        if (_invitationRepository is InvitationRepository repo)
        {
            await repo.LogAllInvitationsAsync();
        }
        
        // Workaround: If this looks like a default request (no filters set), clear the status filter
        if (string.IsNullOrEmpty(request.TenantId) && 
            string.IsNullOrEmpty(request.TargetTenantId) && 
            string.IsNullOrEmpty(request.Keyword) &&
            request.Status == InvitationStatus.Pending)
        {
            _logger.LogInformation("Detected default search request - clearing Status filter to show all invitations");
            request.Status = null;
        }
        
        var spec = new InvitationsByPaginationFilterSpec(request);
        
        var items = await _invitationRepository.ListAsync(spec, cancellationToken);
        var totalCount = await _invitationRepository.CountAsync(spec, cancellationToken);
        
        _logger.LogInformation("SearchInvitationsAsync results: Found {TotalCount} total items, returning {ItemCount} items for page {PageNumber}", 
            totalCount, items.Count, request.PageNumber);
        
        if (items.Any())
        {
            foreach (var item in items)
            {
                _logger.LogInformation("Found invitation: Id={Id}, Email={Email}, TenantId={TenantId}, TargetTenantId={TargetTenantId}, Status={Status}", 
                    item.Id, item.Email, item.TenantId, item.TargetTenantId, item.Status);
            }
        }
        else
        {
            _logger.LogWarning("No invitations found for current user");
        }
        
        return new PagedList<InvitationDto>(items, request.PageNumber, request.PageSize, totalCount);
    }

    public async Task<List<UserInvitation>> GetPendingInvitationsAsync(string? targetTenantId = null, CancellationToken cancellationToken = default)
    {
        return await _invitationRepository.GetPendingAsync(targetTenantId, cancellationToken);
    }

    public async Task<List<UserInvitation>> GetInvitationsByTenantAsync(string targetTenantId, CancellationToken cancellationToken = default)
    {
        return await _invitationRepository.GetByTenantAsync(targetTenantId, cancellationToken);
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
                        TenantId = invitation.TargetTenantId,
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

    public async Task<bool> IsEmailInvitedAsync(string email, string? targetTenantId = null, CancellationToken cancellationToken = default)
    {
        ISpecification<UserInvitation> spec;
        if (!string.IsNullOrEmpty(targetTenantId))
        {
            spec = new InvitationsByEmailAndTenantSpec(email, targetTenantId);
        }
        else
        {
            spec = new InvitationsByEmailSpec(email);
        }

        return await _invitationRepository.AnyAsync(spec, cancellationToken);
    }

    public async Task<bool> HasPendingInvitationAsync(string email, string targetTenantId, CancellationToken cancellationToken = default)
    {
        return await _invitationRepository.ExistsActiveInvitationAsync(email, targetTenantId, cancellationToken);
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