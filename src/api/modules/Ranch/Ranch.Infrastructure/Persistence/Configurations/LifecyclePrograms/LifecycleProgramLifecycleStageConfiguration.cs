using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Persistence.Configurations.LifecyclePrograms;
internal sealed class LifecycleProgramLifecycleStageConfiguration : IEntityTypeConfiguration<LifecycleProgramLifecycleStage>
{
    public void Configure(EntityTypeBuilder<LifecycleProgramLifecycleStage> builder)
    {
        builder
            .HasKey(lps => new {lps.LifecycleProgramId,lps.LifecycleStageId});

        builder
            .HasOne(lps => lps.LifecycleStage)
            .WithMany()
            .HasForeignKey(lps => lps.LifecycleStageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<LifecycleProgram>()
            .WithMany(lp => lp.LifecycleProgramLifecycleStages)
            .HasForeignKey(lps => lps.LifecycleProgramId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Navigation(lps => lps.LifecycleStage)
            .AutoInclude();
    }
}
