using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using HttpServer.Attributes;
using HttpServer.Models;
using HttpServer.ServerLogic.SessionLogic;

namespace HttpServer.ServerLogic;

internal static partial class ServerResponseProvider
{
    public static HttpListenerResponse GetResponse(string path, HttpListenerContext ctx)
    {
        var request = ctx.Request;
        var response = ctx.Response;
        var rawUrl = request.RawUrl ?? string.Empty;
        var buffer = Encoding.UTF8.GetBytes("Error 404. Not Found.");
        if (!Directory.Exists(path))
            buffer = Encoding.UTF8.GetBytes($"Directory {path} not found.");
        else if (TryGetFile(path + rawUrl.Replace("%20", " "), request, response))
            return response;
        else if (TryHandleController(request, response))
            return response;
        FillResponse(response, "text/plain", (int)HttpStatusCode.NotFound, buffer);
        return response;
    }
    
    private static bool TryGetFile(string filePath, HttpListenerRequest request, HttpListenerResponse response)
    {
        byte[] buffer;
        if (Directory.Exists(filePath) && File.Exists(filePath + "index.html"))
            buffer = File.ReadAllBytes(filePath + "index.html");
        else if (!File.Exists(filePath))
            return false;
        else buffer = File.ReadAllBytes(filePath);
        FillResponse(response, GetContentType(request.RawUrl ?? ""), (int)HttpStatusCode.OK, buffer);
        return true;
    }

    private static string GetContentType(string path)
    {
        var ext = path.Contains('.') ? path.Split('.')[^1] : "html";
        return ContentTypeDictionary.ContainsKey(ext) ? ContentTypeDictionary[ext] : "text/plain";
    }
    
    private static bool TryHandleController(HttpListenerRequest request, HttpListenerResponse response)
    {
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
                        .GetField("MethodUri")?
                        .GetValue(attr)?.ToString() ?? "")));
        if (method is null) return false;

        var queryParams = method.GetParameters()
            .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
            .ToArray();
        
        var ret = method.Invoke(Activator.CreateInstance(controller!), queryParams);
        if (!HandleCookies(request, response, method.Name, ret))
            return true;
        
        var buffer = Encoding.ASCII.GetBytes(JsonSerializer.Serialize(ret));
        var statusCode = request.HttpMethod == "POST" ? HttpStatusCode.Redirect : HttpStatusCode.OK;
        FillResponse(response, "application/json", (int)statusCode, buffer);
        return true;
    }

    private static bool HandleCookies(HttpListenerRequest request, HttpListenerResponse response, string methodName, object? ret)
    {
        var sessionCookie = request.Cookies["SessionId"]?.Value ?? "";
        switch (methodName)
        {
            case "Login":
                var retParsed = ((int, Guid))(ret ?? (-1, Guid.Empty));
                if (retParsed == (-1, Guid.Empty))
                    return true;
                response.Cookies.Add(new Cookie("SessionId", retParsed.Item2.ToString()));
                Console.WriteLine();
                return false;
            case "GetAccounts":
                if (SessionManager.CheckSession(Guid.Parse(sessionCookie)))
                    return true;
                FillResponse(response, "text/plain", (int)HttpStatusCode.Unauthorized, Array.Empty<byte>());
                return false;
            case "GetAccountInfo":
                var currentSession = SessionManager.GetSessionInfo(Guid.Parse(sessionCookie));
                if (ret is not null && currentSession is not null && currentSession.AccountId == ((Account)ret).Id)
                    return true;
                FillResponse(response, "text/plain", (int)HttpStatusCode.Unauthorized, Array.Empty<byte>());
                return false;
            default:
                return true;
        }
    }
    

    private static void FillResponse(HttpListenerResponse response, string contentType, int statusCode, byte[] buffer)
    {
        response.Headers.Set("Content-Type", contentType);
        response.StatusCode = statusCode;
        response.OutputStream.Write(buffer, 0, buffer.Length);
    }
}