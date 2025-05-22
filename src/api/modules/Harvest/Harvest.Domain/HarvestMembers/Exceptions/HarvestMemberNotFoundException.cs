using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Harvest.Domain.HarvestMembers.Exceptions;
public sealed class HarvestMemberNotFoundException : NotFoundException
{
    public HarvestMemberNotFoundException(Guid id)
        : base($"harvestMember with id {id} not found")
    {
    }
}

