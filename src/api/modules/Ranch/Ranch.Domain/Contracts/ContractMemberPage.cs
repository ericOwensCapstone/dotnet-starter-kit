using FSH.Framework.Core.Tenant.Abstractions;
using FSH.Starter.WebApi.Ranch.Domain.MemberPages;
namespace FSH.Starter.WebApi.Ranch.Domain.Contracts;

                    public class ContractMemberPage
                    {
                        public Guid ContractId { get; set; }
                        public Guid MemberPageId { get; set; }
    //TODO Shared
                        [PropertyWithMemberId]
                        public MemberPage MemberPage { get; set; } = default!;
                    }
                    
