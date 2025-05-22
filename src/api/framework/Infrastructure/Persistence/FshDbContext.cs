using System.Linq.Expressions;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Tenant.Abstractions;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.Shared.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Npgsql;

namespace FSH.Framework.Infrastructure.Persistence;
public class FshDbContext(IMultiTenantContextAccessor<FshTenantInfo> multiTenantContextAccessor,
    DbContextOptions options,
    IPublisher publisher,
    IOptions<DatabaseOptions> settings)
    : MultiTenantDbContext(multiTenantContextAccessor, options)
{
    private readonly IPublisher _publisher = publisher;
    private readonly DatabaseOptions _settings = settings.Value;

    //TODO Member Start - can we switch all uses of TenantId below to MemberId?
    // except we would set both TenantId and MemberId in the save of added entities?

    private string GetCurrentTenantId()
    {
        var currentTenantId = multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id ?? throw new InvalidOperationException("TenantId is not set.");
        return currentTenantId;
    }

    private Guid GetCurrentMemberId()
    {
        var currentMemberId = multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.MemberId ?? throw new InvalidOperationException("TenantId is not set.");
        return currentMemberId;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var entityClrType = entityType.ClrType;
            var parameter = Expression.Parameter(entityClrType, "e");

            Expression tenantFilter = null;
            Expression sharedWithFilter = null;
            Expression softDeleteFilter = null;

            // Tenant filter
            if (typeof(ITenantEntity).IsAssignableFrom(entityClrType))
            {
                if (!typeof(IPublicEntity).IsAssignableFrom(entityClrType))
                {
                    var tenantIdProperty = Expression.Property(parameter, nameof(ITenantEntity.TenantId));
                    var tenantIdMethod = typeof(FshDbContext).GetMethod(nameof(GetCurrentTenantId), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    var tenantIdCall = Expression.Call(Expression.Constant(this), tenantIdMethod);
                    tenantFilter = Expression.Equal(tenantIdProperty, tenantIdCall);
                }
            }

            // SharedWith filter (generic)
            if (typeof(IShareableEntity).IsAssignableFrom(entityClrType))
            {
                var sharedWithProp = entityClrType
                    .GetProperties()
                    .FirstOrDefault(p => Attribute.IsDefined(p, typeof(SharedWithMembersAttribute)));

                if (sharedWithProp != null)
                {
                    var sharedWithProperty = Expression.Property(parameter, sharedWithProp.Name);
                    var elementType = sharedWithProp.PropertyType.GetGenericArguments().First();

                    // Find the property with [PropertyWithMemberId]
                    var memberIdNavProp = elementType.GetProperties()
                        .FirstOrDefault(p => Attribute.IsDefined(p, typeof(PropertyWithMemberIdAttribute)));

                    if (memberIdNavProp != null)
                    {
                        var xParam = Expression.Parameter(elementType, "x");
                        var memberPageProperty = Expression.Property(xParam, memberIdNavProp.Name);

                        // Now get the MemberId property from the navigation property
                        var memberIdProperty = Expression.Property(memberPageProperty, "MemberId");

                        var memberIdMethod = typeof(FshDbContext).GetMethod(nameof(GetCurrentMemberId), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        var memberIdCall = Expression.Call(Expression.Constant(this), memberIdMethod);

                        var memberIdCallConverted = Expression.Convert(memberIdCall, typeof(Guid?));
                        var memberIdEquals = Expression.Equal(memberIdProperty, memberIdCallConverted);
                        var lambda = Expression.Lambda(memberIdEquals, xParam);

                        var anyMethod = typeof(Enumerable).GetMethods()
                            .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                            .MakeGenericMethod(elementType);

                        sharedWithFilter = Expression.Call(anyMethod, sharedWithProperty, lambda);
                    }
                }
            }


            // Soft delete filter
            if (typeof(ISoftDeletable).IsAssignableFrom(entityClrType))
            {
                softDeleteFilter = Expression.Equal(
                    Expression.Property(parameter, nameof(ISoftDeletable.Deleted)),
                    Expression.Constant(null));
            }

            // Combine filters: (tenantFilter OR sharedWithFilter) AND softDeleteFilter
            Expression accessFilter = null;
            if (tenantFilter != null && sharedWithFilter != null)
                accessFilter = Expression.OrElse(tenantFilter, sharedWithFilter);
            else
                accessFilter = tenantFilter ?? sharedWithFilter;

            Expression combinedFilter = null;
            if (accessFilter != null && softDeleteFilter != null)
                combinedFilter = Expression.AndAlso(accessFilter, softDeleteFilter);
            else
                combinedFilter = accessFilter ?? softDeleteFilter;

            if (combinedFilter != null)
            {
                var lambda = Expression.Lambda(combinedFilter, parameter);
                entityType.SetQueryFilter(lambda);
            }
        }

        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();

        if (!string.IsNullOrWhiteSpace(multiTenantContextAccessor?.MultiTenantContext.TenantInfo?.ConnectionString))
        {
            optionsBuilder.ConfigureDatabase(_settings.Provider, multiTenantContextAccessor.MultiTenantContext.TenantInfo.ConnectionString!);
        }
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Id;
        var memberId = multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.MemberId;

        if (string.IsNullOrEmpty(tenantId))
        {
            throw new InvalidOperationException("TenantId is not set.");
        }

        if (memberId == null || memberId == Guid.Empty)
        {
            throw new InvalidOperationException("MemberId is not set.");
        }

        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            if (entry.Entity is ITenantEntity tenantEntity && entry.State == EntityState.Added)
            {
                // Set the TenantId for new entities
                tenantEntity.TenantId = tenantId;
                // Set the MemberId for new entities
                tenantEntity.MemberId = memberId;
            }
            else if (entry.Entity is ITenantEntity tenantEntity2 && entry.State == EntityState.Modified)
            {
                if (tenantEntity2.TenantId != tenantId)
                {
                    var entityClrType = entry.Entity.GetType();
                    throw new UnauthorizedAccessException($"You cannot modify another tenant's {entityClrType.Name} entity.");
                }
                // Prevent TenantId from being modified
                entry.Property(nameof(ITenantEntity.TenantId)).IsModified = false;
            }
            else if (entry.Entity is ITenantEntity tenantEntity3 && entry.State == EntityState.Deleted)
            {
                // Special case: Allow root tenant to delete any MemberPage or HarvestMember
                bool isRootTenant = tenantId == TenantConstants.Root.Id;
                bool isMemberPage = entry.Entity.GetType().Name == "MemberPage";
                bool isHarvestMember = entry.Entity.GetType().Name == "HarvestMember";
                
                if (tenantEntity3.TenantId != tenantId && !(isRootTenant && (isMemberPage || isHarvestMember)))
                {
                    var entityClrType = entry.Entity.GetType();
                    throw new UnauthorizedAccessException($"You cannot delete another tenant's {entityClrType.Name} entity.");
                }
            }

            if (entry.Entity is ISoftDeletable softDeletable && entry.State == EntityState.Deleted)
            {
                var (canBeDeleted, reason) = softDeletable.CanBeSoftDeleted(this);
                if (!canBeDeleted)
                {
                    throw new InvalidOperationException(reason);
                }             
            }
        }

        try
        {
            this.TenantNotSetMode = TenantNotSetMode.Overwrite;
            int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await PublishDomainEventsAsync().ConfigureAwait(false);
            return result;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            throw new DuplicateEntityNameException();
        }
    }
    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker.Entries<IEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Count > 0)
            .SelectMany(e =>
            {
                var domainEvents = e.DomainEvents.ToList();
                e.DomainEvents.Clear();
                return domainEvents;
            })
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent).ConfigureAwait(false);
        }
    }
}
