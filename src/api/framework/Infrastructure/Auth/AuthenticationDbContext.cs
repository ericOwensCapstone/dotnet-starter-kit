using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FSH.Framework.Infrastructure.Identity.Users;
using FSH.Framework.Infrastructure.Identity.Roles;
using FSH.Framework.Infrastructure.Identity.RoleClaims;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Framework.Core.Identity.Invitations;
using Shared.Constants;

namespace FSH.Framework.Infrastructure.Auth;

/// <summary>
/// A specialized DbContext for authentication purposes that doesn't require tenant context.
/// Used by B2C authentication to query users during the authentication process.
/// </summary>
public class AuthenticationDbContext : DbContext
{
    public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
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

        // Configure the Identity tables with the correct schema
        modelBuilder.Entity<FshUser>().ToTable("Users", SchemaNames.Identity);
        modelBuilder.Entity<FshRole>().ToTable("Roles", SchemaNames.Identity);
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", SchemaNames.Identity);
        modelBuilder.Entity<FshRoleClaim>().ToTable("RoleClaims", SchemaNames.Identity);
        modelBuilder.Entity<FshTenantInfo>().ToTable("Tenants", SchemaNames.Tenant);
        modelBuilder.Entity<UserInvitation>().ToTable("UserInvitations", SchemaNames.Identity);

        // Configure the composite key for UserRoles
        modelBuilder.Entity<IdentityUserRole<string>>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        // Configure relationships
        modelBuilder.Entity<IdentityUserRole<string>>()
            .HasOne<FshUser>()
            .WithMany()
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        modelBuilder.Entity<IdentityUserRole<string>>()
            .HasOne<FshRole>()
            .WithMany()
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();
            
        // Configure UserInvitation without multi-tenancy
        modelBuilder.Entity<UserInvitation>(builder =>
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Email)
                .HasMaxLength(254)
                .IsRequired();
                
            builder.Property(x => x.InvitationToken)
                .HasMaxLength(100)
                .IsRequired();
                
            builder.HasIndex(x => x.InvitationToken)
                .IsUnique();
        });
    }
}