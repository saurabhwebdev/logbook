using System.Net;

namespace CoreEngine.Shared.Exceptions;

public class CoreEngineException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public CoreEngineException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public CoreEngineException(string message, Exception innerException, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
