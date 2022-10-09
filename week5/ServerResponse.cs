using System.Net;

namespace HttpServer;

public class ServerResponse
{
    public readonly byte[] Buffer;
    public readonly string ContentType;

    public ServerResponse(byte[] buffer, string contentType)
    {
        Buffer = buffer;
        ContentType = contentType;
    }
}