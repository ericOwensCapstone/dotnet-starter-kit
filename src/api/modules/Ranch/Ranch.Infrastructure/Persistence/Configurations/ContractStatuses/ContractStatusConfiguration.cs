using Finbuckle.MultiTenant;
using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Persistence.Configurations.ContractStatuses;
internal sealed class ContractStatusConfiguration : IEntityTypeConfiguration<ContractStatus>
{
    public void Configure(EntityTypeBuilder<ContractStatus> builder)
    {
        builder.Property<DateTimeOffset?>("Deleted"); // Define Deleted as a shadow property  
        builder.HasIndex("Name", "TenantId").IsUnique().HasFilter("\"Deleted\" IS NULL");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(99);
        builder.Property(x => x.Description).HasMaxLength(999);
    }
}