using FSH.Framework.Core.Tenant.Abstractions;
using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers;
namespace FSH.Starter.WebApi.Harvest.Domain.HarvestContracts;

                public class HarvestContractHarvestMember
                {
                    public Guid HarvestContractId { get; set; }
                    public Guid HarvestMemberId { get; set; }
                    [PropertyWithMemberId]
                    public HarvestMember HarvestMember { get; set; } = default!;
                }
                
