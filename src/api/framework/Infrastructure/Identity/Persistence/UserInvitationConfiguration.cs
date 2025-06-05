using Finbuckle.MultiTenant;
using FSH.Framework.Core.Identity.Invitations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IdentityConstants = FSH.Starter.Shared.Authorization.IdentityConstants;

namespace FSH.Framework.Infrastructure.Identity.Persistence;

public class UserInvitationConfiguration : IEntityTypeConfiguration<UserInvitation>
{
    public void Configure(EntityTypeBuilder<UserInvitation> builder)
    {
        builder.ToTable("UserInvitations", IdentityConstants.SchemaName)
            .IsMultiTenant();

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
    }
}