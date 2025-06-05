using FSH.Framework.Core.Audit;
using FSH.Framework.Infrastructure.Identity.Persistence;
using FSH.Framework.Infrastructure.Persistence.Interceptors;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Framework.Infrastructure.Identity.Audit;
public class AuditPublishedEventHandler(ILogger<AuditPublishedEventHandler> logger, IdentityDbContext context) : INotificationHandler<AuditPublishedEvent>
{
    private static readonly ThreadLocal<bool> _isSavingAuditTrails = new(() => false);
    
    public async Task Handle(AuditPublishedEvent notification, CancellationToken cancellationToken)
    {
        if (context == null) return;
        
        // Prevent infinite recursion when saving audit trails
        if (_isSavingAuditTrails.Value)
        {
            logger.LogDebug("Skipping audit trail save to prevent infinite recursion");
            return;
        }
        
        logger.LogInformation("received audit trails");
        try
        {
            _isSavingAuditTrails.Value = true;
            await context.Set<AuditTrail>().AddRangeAsync(notification.Trails!, default);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            logger.LogError("error while saving audit trail");
        }
        finally
        {
            _isSavingAuditTrails.Value = false;
        }
        return;
    }
}
