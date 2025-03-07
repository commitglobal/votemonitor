using System.Net;

namespace Vote.Monitor.Core.Exceptions;

public class InternalServerException : CustomException
{
    public InternalServerException(string message, List<string>? errors = null)
        : base(message, errors, HttpStatusCode.InternalServerError)
    {
    }
}
