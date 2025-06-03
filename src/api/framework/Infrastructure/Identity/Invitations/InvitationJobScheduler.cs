using FSH.Framework.Core.Jobs;
using FSH.Framework.Core.Persistence;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FSH.Framework.Infrastructure.Identity.Invitations;

public static class InvitationJobScheduler
{
    public static void ScheduleInvitationJobs(this IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<ProcessExpiredInvitationsJob>>();
        logger.LogInformation("Scheduling invitation-related jobs...");

        // Try to clear any stale locks first
        TryClearStaleLocks(serviceProvider, logger);

        // Add retry logic for job scheduling to handle potential lock timeouts
        var maxRetries = 3;
        var delay = TimeSpan.FromSeconds(5);
        
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                // Schedule job to process expired invitations every hour
                RecurringJob.AddOrUpdate<ProcessExpiredInvitationsJob>(
                    "process-expired-invitations",
                    job => job.ProcessAsync(CancellationToken.None),
                    Cron.Hourly);

                logger.LogInformation("Invitation jobs scheduled successfully.");
                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to schedule invitation jobs (attempt {Attempt}/{MaxRetries})", attempt, maxRetries);
                
                if (attempt < maxRetries)
                {
                    logger.LogInformation("Retrying job scheduling in {Delay} seconds...", delay.TotalSeconds);
                    Thread.Sleep(delay);
                    delay = TimeSpan.FromSeconds(delay.TotalSeconds * 2); // Exponential backoff
                }
                else
                {
                    logger.LogWarning(ex, "Failed to schedule invitation jobs after {MaxRetries} attempts. Jobs will be scheduled on first use.", maxRetries);
                    // Don't throw - allow the application to continue running
                    return;
                }
            }
        }
    }

    private static void TryClearStaleLocks(IServiceProvider serviceProvider, ILogger logger)
    {
        try
        {
            var dbOptions = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<DatabaseOptions>>().Value;
            
            // Only try to clear locks for PostgreSQL
            if (dbOptions.Provider.Equals("postgresql", StringComparison.OrdinalIgnoreCase))
            {
                using var connection = new NpgsqlConnection(dbOptions.ConnectionString);
                connection.Open();

                // Clear stale Hangfire locks (older than 5 minutes)
                var clearLocksQuery = @"
                    DELETE FROM hangfire.lock 
                    WHERE resource LIKE '%process-expired-invitations%' 
                    AND acquired < NOW() - INTERVAL '5 minutes'";

                using var command = new NpgsqlCommand(clearLocksQuery, connection);
                var deletedRows = command.ExecuteNonQuery();
                
                if (deletedRows > 0)
                {
                    logger.LogInformation("Cleared {DeletedRows} stale Hangfire locks for invitation jobs", deletedRows);
                }
            }
        }
        catch (Exception ex)
        {
            // Don't fail the entire operation if we can't clear locks
            logger.LogDebug(ex, "Could not clear stale locks, continuing with job scheduling...");
        }
    }
}