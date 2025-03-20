using System.Net;

namespace FSH.Framework.Core.Exceptions;
public class DuplicateEntityNameException : FshException
{
    public DuplicateEntityNameException()
        : base("An entity with that name already exists.", [], HttpStatusCode.Conflict)
    {
    }
    public DuplicateEntityNameException(string message)
        : base(message, [], HttpStatusCode.Conflict)
    {
    }
}

