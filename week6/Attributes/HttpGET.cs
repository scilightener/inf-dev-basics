namespace HttpServer.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
internal class HttpGET : Attribute
{
    public readonly string MethodURI;

    public HttpGET(string methodUri = "")
    {
        MethodURI = methodUri;
    }
}