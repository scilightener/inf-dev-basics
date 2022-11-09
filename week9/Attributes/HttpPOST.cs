namespace HttpServer.Attributes;

internal class HttpPOST : Attribute
{
    public readonly string MethodUri;

    public HttpPOST(string methodUri = "")
    {
        MethodUri = methodUri;
    }
}