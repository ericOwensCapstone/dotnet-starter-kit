using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FSH.Framework.Infrastructure.Identity.Users;
using FSH.Framework.Infrastructure.Identity.Roles;
using FSH.Framework.Infrastructure.Identity.RoleClaims;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Framework.Core.Identity.Invitations;
using Shared.Constants;

namespace FSH.Framework.Infrastructure.Persistence;

/// <summary>
/// DbContext for operations that need to bypass multi-tenant filtering.
/// Used for B2C authentication endpoints and root admin cross-tenant operations.
/// </summary>
public class TenantFreeDbContext : DbContext
{
    public TenantFreeDbContext(DbContextOptions<TenantFreeDbContext> options)
        : base(options)
    {
    }

    public DbSet<FshUser> Users => Set<FshUser>();
    public DbSet<FshRole> Roles => Set<FshRole>();
    public DbSet<IdentityUserRole<string>> UserRoles => Set<IdentityUserRole<string>>();
    public DbSet<FshRoleClaim> RoleClaims => Set<FshRoleClaim>();
    public DbSet<FshTenantInfo> Tenants => Set<FshTenantInfo>();
    public DbSet<UserInvitation> UserInvitations => Set<UserInvitation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Users table
        modelBuilder.Entity<FshUser>(entity =>
        {
            entity.ToTable("Users", SchemaNames.Identity);
            entity.Property(u => u.ObjectId).HasMaxLength(450);
            
            // Add TenantId shadow property
            entity.Property<string>("TenantId").HasMaxLength(64);
            entity.HasIndex("TenantId");
        });

        // Configure Roles table
        modelBuilder.Entity<FshRole>(entity =>
        {
            entity.ToTable("Roles", SchemaNames.Identity);
            entity.Property(r => r.Name).HasMaxLength(256);
            entity.Property(r => r.NormalizedName).HasMaxLength(256);
            
            // Add TenantId shadow property
            entity.Property<string>("TenantId").HasMaxLength(64);
            entity.HasIndex("TenantId");
        });

        // Configure UserRoles table with composite key
        modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UserRoles", SchemaNames.Identity);
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.HasOne<FshUser>()
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<FshRole>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Add TenantId shadow property
            entity.Property<string>("TenantId").HasMaxLength(64);
            entity.HasIndex("TenantId");
        });

        // Configure RoleClaims table
        modelBuilder.Entity<FshRoleClaim>(entity =>
        {
            entity.ToTable("RoleClaims", SchemaNames.Identity);

            entity.HasOne<FshRole>()
                .WithMany()
                .HasForeignKey(rc => rc.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Add TenantId shadow property
            entity.Property<string>("TenantId").HasMaxLength(64);
            entity.HasIndex("TenantId");
        });

        // Configure Tenants table
        modelBuilder.Entity<FshTenantInfo>(entity =>
        {
            entity.ToTable("Tenants", SchemaNames.Tenant);
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Id).HasMaxLength(64);
            entity.Property(t => t.Identifier).HasMaxLength(450);
            entity.Property(t => t.Name).HasMaxLength(128);
            entity.Property(t => t.AdminEmail).HasMaxLength(256);

            entity.HasIndex(t => t.Identifier).IsUnique();
        });

        // Configure UserInvitations table
        modelBuilder.Entity<UserInvitation>(entity =>
        {
            entity.ToTable("UserInvitations", SchemaNames.Identity);
            
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(254);

            entity.Property(e => e.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.FirstName)
                .HasMaxLength(50);

            entity.Property(e => e.LastName)
                .HasMaxLength(50);

            entity.Property(e => e.InvitationToken)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.InvitedBy)
                .IsRequired()
                .HasMaxLength(254);

            entity.Property(e => e.Role)
                .HasMaxLength(50);

            entity.Property(e => e.TargetTenantId)
                .IsRequired()
                .HasMaxLength(64);

            entity.Property(e => e.TenantId)
                .HasMaxLength(64);

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(e => e.B2CUserId)
                .HasMaxLength(100);

            entity.Property(e => e.ErrorMessage)
                .HasMaxLength(1000);

            entity.Property(e => e.AcceptedByUserId)
                .HasMaxLength(450);

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(128);

            entity.Property(e => e.LastModifiedBy)
                .HasMaxLength(128);

            entity.HasIndex(e => e.InvitationToken)
                .IsUnique();

            entity.HasIndex(e => e.Email);
            
            entity.HasIndex(e => e.TargetTenantId);
            
            entity.HasIndex(e => e.Status);
            
            entity.HasIndex(e => new { e.Email, e.TargetTenantId });
            
            entity.HasIndex(e => e.ExpiresAt);
        });
    }
}