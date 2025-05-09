using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Members.Domain.MemberAds.Exceptions;
public sealed class MemberAdNotFoundException : NotFoundException
{
    public MemberAdNotFoundException(Guid id)
        : base($"memberAd with id {id} not found")
    {
    }
}

