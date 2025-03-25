using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Persistence.Configurations.LifecycleStages;
internal sealed class LifecycleStageConfiguration : IEntityTypeConfiguration<LifecycleStage>
{
    public void Configure(EntityTypeBuilder<LifecycleStage> builder)
    {
        builder.IsMultiTenant();
        builder.Property<string>("TenantId"); // Define TenantId as a shadow property
        builder.Property<DateTimeOffset?>("Deleted"); // Define Deleted as a shadow property  
        builder.HasIndex("Name", "TenantId").IsUnique().HasFilter("\"Deleted\" IS NULL");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(99);
        builder.Property(x => x.Description).HasMaxLength(999);
        builder.HasOne(l => l.Ration)
               .WithMany()
               .HasForeignKey(l => l.RationId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(ls => ls.Ration).AutoInclude();
        builder.HasOne(l => l.GrowthTreatment)
               .WithMany()
               .HasForeignKey(l => l.GrowthTreatmentId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(ls => ls.GrowthTreatment).AutoInclude();
        builder.HasOne(l => l.PreventiveTreatment)
               .WithMany()
               .HasForeignKey(l => l.PreventiveTreatmentId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.Navigation(ls => ls.PreventiveTreatment).AutoInclude();
    }
}