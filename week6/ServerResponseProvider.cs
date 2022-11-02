using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using HttpServer.Attributes;

namespace HttpServer;

internal static partial class ServerResponseProvider
{
    public static ServerResponse GetResponse(string path, HttpListenerRequest request)
    {
        var rawUrl = request.RawUrl ?? string.Empty;
        if (!Directory.Exists(path))
            return new ServerResponse(
                Encoding.UTF8.GetBytes($"Directory {path} not found."),
                "text/plain",
                HttpStatusCode.NotFound);

        if (TryGetFile(path + rawUrl.Replace("%20", " "), out var buffer))
            return new ServerResponse(buffer, GetContentType(rawUrl), HttpStatusCode.OK);

        if (TryHandleController(request, out buffer))
        {
            var statusCode = buffer.Length > 0 ? HttpStatusCode.OK : HttpStatusCode.Redirect;
            return new ServerResponse(buffer, "application/json", statusCode);
        }
        
        buffer = Encoding.UTF8.GetBytes("Error 404. Not Found.");
        return new ServerResponse(buffer, "text/plain", HttpStatusCode.NotFound);
    }
    
    private static bool TryGetFile(string filePath, out byte[] buffer)
    {
        buffer = Array.Empty<byte>();
    
        if (Directory.Exists(filePath) && File.Exists(filePath + "index.html"))
        {
            buffer = File.ReadAllBytes(filePath + "index.html");
            return true;
        }

        if (!File.Exists(filePath)) return false;
        buffer = File.ReadAllBytes(filePath);
        return true;
    }

    private static string GetContentType(string path)
    {
        var ext = path.Contains('.') ? path.Split('.')[^1] : "html";
        return ContentTypeDictionary.ContainsKey(ext) ? ContentTypeDictionary[ext] : "text/plain";
    }
    
    private static bool TryHandleController(HttpListenerRequest request, out byte[] buffer)
    {
        buffer = Array.Empty<byte>();
        if (request.Url!.Segments.Length < 2) return false;
        
        using var sr = new StreamReader(request.InputStream, request.ContentEncoding);
        var bodyParam = sr.ReadToEnd();
        
        var controllerName = request.Url.Segments[1].Replace("/", "");
        var strParams = request.Url.Segments
            .Skip(2)
            .Select(s => s.Replace("/", ""))
            .Concat(bodyParam.Split('&').Select(p => p.Split('=').LastOrDefault()))
            .ToArray();

        var assembly = Assembly.GetExecutingAssembly();
        var controller = assembly.GetTypes()
            .Where(t => Attribute.IsDefined(t, typeof(HttpController)))
            .FirstOrDefault(t => string.Equals(
                (t.GetCustomAttribute(typeof(HttpController)) as HttpController)?.ControllerName,
                controllerName,
                StringComparison.CurrentCultureIgnoreCase));

        var method = controller?.GetMethods()
            .FirstOrDefault(t => t.GetCustomAttributes(true)
                .Any(attr => attr.GetType().Name == $"Http{request.HttpMethod}"
                    && Regex.IsMatch(request.RawUrl ?? "",
                        attr.GetType()
                        .GetField("MethodURI")?
                        .GetValue(attr)?.ToString() ?? "")));
        if (method is null) return false;

        var queryParams = method.GetParameters()
            .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
            .ToArray();
        
        var ret = method.Invoke(Activator.CreateInstance(controller!), queryParams);
        buffer = request.HttpMethod == "POST" ? buffer : Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
        
        return true;
    }
}