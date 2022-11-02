using System.Net;

namespace HttpServer;

public class ServerResponse
{
    public readonly byte[] Buffer;
    public readonly string ContentType;
    public readonly HttpStatusCode ResponseCode;

    public ServerResponse(byte[] buffer, string contentType, HttpStatusCode responseCode)
    {
        Buffer = buffer;
        ContentType = contentType;
        ResponseCode = responseCode;
    }
}