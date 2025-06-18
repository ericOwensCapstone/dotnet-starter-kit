using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Identity.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FSH.Framework.Infrastructure.Identity.Invitations;

/// <summary>
/// Repository for anonymous access to invitations (e.g., validation endpoints)
/// This bypasses multi-tenant filtering to allow invitation validation without authentication
/// </summary>
public class AnonymousInvitationRepository : IAnonymousInvitationRepository
{
    private readonly DbContextOptions<IdentityDbContext> _dbContextOptions;
    private readonly DatabaseOptions _databaseOptions;
    private readonly ILogger<AnonymousInvitationRepository>? _logger;

    public AnonymousInvitationRepository(
        DbContextOptions<IdentityDbContext> dbContextOptions,
        IOptions<DatabaseOptions> databaseOptions,
        ILogger<AnonymousInvitationRepository>? logger = null)
    {
        _dbContextOptions = dbContextOptions;
        _databaseOptions = databaseOptions.Value;
        _logger = logger;
    }

    public async Task<UserInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(token))
        {
            _logger?.LogWarning("GetByTokenAsync called with null or empty token");
            return null;
        }

        try
        {
            _logger?.LogInformation("AnonymousInvitationRepository.GetByTokenAsync: Looking for invitation with token: {Token}", token);
            
            // Create a new DbContext without multi-tenant context
            using var context = new AnonymousIdentityDbContext(_dbContextOptions, _databaseOptions);
            
            var invitation = await context.UserInvitations
                .Where(x => x.InvitationToken == token)
                .FirstOrDefaultAsync(cancellationToken);
                
            if (invitation == null)
            {
                _logger?.LogWarning("AnonymousInvitationRepository.GetByTokenAsync: No invitation found for token: {Token}", token);
            }
            else
            {
                _logger?.LogInformation("AnonymousInvitationRepository.GetByTokenAsync: Found invitation - Id: {Id}, Email: {Email}, Status: {Status}",
                    invitation.Id, invitation.Email, invitation.Status);
            }
            
            return invitation;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving invitation by token: {Token}", token);
            throw;
        }
    }

    public async Task<UserInvitation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            using var context = new AnonymousIdentityDbContext(_dbContextOptions, _databaseOptions);
            return await context.UserInvitations
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving invitation by id: {Id}", id);
            throw;
        }
    }

    public async Task<UserInvitation?> FirstOrDefaultAsync(ISpecification<UserInvitation> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            using var context = new AnonymousIdentityDbContext(_dbContextOptions, _databaseOptions);
            return await context.UserInvitations
                .WithSpecification(specification)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving invitation by specification");
            throw;
        }
    }

    public async Task UpdateAsync(UserInvitation invitation, CancellationToken cancellationToken = default)
    {
        try
        {
            using var context = new AnonymousIdentityDbContext(_dbContextOptions, _databaseOptions);
            context.UserInvitations.Update(invitation);
            await context.SaveChangesAsync(cancellationToken);
            
            _logger?.LogInformation("Updated invitation {Id} with status {Status}", invitation.Id, invitation.Status);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating invitation: {Id}", invitation.Id);
            throw;
        }
    }

    public async Task<List<UserInvitation>> ListAsync(ISpecification<UserInvitation> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            using var context = new AnonymousIdentityDbContext(_dbContextOptions, _databaseOptions);
            return await context.UserInvitations
                .WithSpecification(specification)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving invitations by specification");
            throw;
        }
    }
}

/// <summary>
/// A simplified DbContext for anonymous access that doesn't use multi-tenancy
/// </summary>
internal class AnonymousIdentityDbContext : DbContext
{
    private readonly DatabaseOptions _databaseOptions;

    public AnonymousIdentityDbContext(DbContextOptions<IdentityDbContext> options, DatabaseOptions databaseOptions) 
        : base(CreateOptionsForProvider(databaseOptions))
    {
        _databaseOptions = databaseOptions;
    }

    public DbSet<UserInvitation> UserInvitations => Set<UserInvitation>();

    private static DbContextOptions CreateOptionsForProvider(DatabaseOptions databaseOptions)
    {
        var optionsBuilder = new DbContextOptionsBuilder();
        
        switch (databaseOptions.Provider?.ToUpperInvariant())
        {
            case DbProviders.PostgreSQL:
                optionsBuilder.UseNpgsql(databaseOptions.ConnectionString);
                break;
            case DbProviders.MSSQL:
                optionsBuilder.UseSqlServer(databaseOptions.ConnectionString);
                break;
            default:
                throw new InvalidOperationException($"Unsupported database provider: {databaseOptions.Provider}");
        }
        
        return optionsBuilder.Options;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure UserInvitation without multi-tenant filtering
        // Match the exact configuration from UserInvitationConfiguration but without IsMultiTenant()
        modelBuilder.Entity<UserInvitation>(builder =>
        {
            builder.ToTable("UserInvitations", "identity");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                .HasMaxLength(254)
                .IsRequired();

            builder.Property(x => x.DisplayName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.FirstName)
                .HasMaxLength(50);

            builder.Property(x => x.LastName)
                .HasMaxLength(50);

            // Ownership properties (for data privacy)
            builder.Property(x => x.TenantId)
                .HasMaxLength(64);

            builder.Property(x => x.MemberId);

            // Target tenant for the invitation
            builder.Property(x => x.TargetTenantId)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(x => x.InvitedBy)
                .HasMaxLength(254)
                .IsRequired();

            builder.Property(x => x.Role)
                .HasMaxLength(50);

            // IMPORTANT: Configure Status enum as string
            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.InvitationToken)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.B2CUserId)
                .HasMaxLength(100);

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(1000);

            builder.Property(x => x.AcceptedByUserId)
                .HasMaxLength(450); // Same as Identity user ID

            // Indexes
            builder.HasIndex(x => x.Email)
                .HasDatabaseName("IX_UserInvitations_Email");

            builder.HasIndex(x => x.TenantId)
                .HasDatabaseName("IX_UserInvitations_TenantId");

            builder.HasIndex(x => x.TargetTenantId)
                .HasDatabaseName("IX_UserInvitations_TargetTenantId");

            builder.HasIndex(x => x.InvitationToken)
                .IsUnique()
                .HasDatabaseName("IX_UserInvitations_InvitationToken");

            builder.HasIndex(x => x.Status)
                .HasDatabaseName("IX_UserInvitations_Status");

            builder.HasIndex(x => new { x.Email, x.TargetTenantId })
                .HasDatabaseName("IX_UserInvitations_Email_TargetTenantId");

            builder.HasIndex(x => x.ExpiresAt)
                .HasDatabaseName("IX_UserInvitations_ExpiresAt");

            // Auditable properties (inherited from AuditableEntity)
            builder.Property(x => x.CreatedBy)
                .HasMaxLength(128);

            builder.Property(x => x.LastModifiedBy)
                .HasMaxLength(128);
        });
    }
}