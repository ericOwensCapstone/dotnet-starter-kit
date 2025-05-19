using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Ranch.Domain.MemberPages.Exceptions;
public sealed class MemberPageNotFoundException : NotFoundException
{
    public MemberPageNotFoundException(Guid id)
        : base($"memberPage with id {id} not found")
    {
    }
}

