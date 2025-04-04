using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Persistence.Configurations.LifecyclePrograms;
internal sealed class LifecycleProgramConfiguration : IEntityTypeConfiguration<LifecycleProgram>
{
    public void Configure(EntityTypeBuilder<LifecycleProgram> builder)
    {
        builder.IsMultiTenant();
        builder.Property<string>("TenantId"); // Define TenantId as a shadow property
        builder.Property<DateTimeOffset?>("Deleted"); // Define Deleted as a shadow property  
        builder.HasIndex("Name", "TenantId").IsUnique().HasFilter("\"Deleted\" IS NULL");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Navigation(lp => lp.LifecycleProgramLifecycleStages).AutoInclude();

    }
}