namespace HttpServer.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
internal class HttpPOST : Attribute
{
    public readonly string MethodURI;

    public HttpPOST(string methodUri = "")
    {
        MethodURI = methodUri;
    }
}