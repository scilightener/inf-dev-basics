namespace HttpServer;
using System.Net;
using System.Text;
using System.Text.Json;

public class HttpServer : IDisposable
{
    public ServerStatus Status = ServerStatus.Stop;
    private ServerSettings _serverSettings;
    
    private readonly HttpListener _httpListener;

    public HttpServer()
    {
        _serverSettings = new ServerSettings();
        _httpListener = new HttpListener();
    }

    public void Start()
    {
        if (Status == ServerStatus.Start)
        {
            Console.WriteLine("Already launched.");
            return;
        }

        if (File.Exists("./settings.json"))
            _serverSettings = JsonSerializer.Deserialize<ServerSettings>(File.ReadAllBytes("./settings.json"))
                ?? new ServerSettings();
        _httpListener.Prefixes.Clear();
        _httpListener.Prefixes.Add($"http://localhost:{_serverSettings.Port}/");
        
        Console.WriteLine("Launching...");
        _httpListener.Start();
        
        Console.WriteLine("Launched.");
        Status = ServerStatus.Start;
        
        Listen();
    }

    public void Stop()
    {
        if (Status == ServerStatus.Stop)
        {
            Console.WriteLine("Already stopped.");
            return;
        }
        
        Console.WriteLine("Stopping...");
        _httpListener.Stop();

        Status = ServerStatus.Stop;
        Console.WriteLine("Stopped.");
    }

    private void Listen()
        => _httpListener.BeginGetContext(ListenerCallback, _httpListener);

    private void ListenerCallback(IAsyncResult result)
    {
        if (!_httpListener.IsListening) return;
        var httpContext = _httpListener.EndGetContext(result);
        var request = httpContext.Request;
        var response = httpContext.Response;
        
        var buffer = Directory.Exists(_serverSettings.Path)
            ? GetFile(request.RawUrl?.Replace("%20", " ") ?? "/")
            : Encoding.UTF8.GetBytes($"Directory {_serverSettings.Path} not found.");

        if (buffer.Length == 0)
        {
            response.Headers.Set("Content-Type", "text/plain");
            response.StatusCode = (int)HttpStatusCode.NotFound;
            buffer = Encoding.UTF8.GetBytes("Error 404. Not Found.");
        }

        var output = response.OutputStream;
        var res = output.WriteAsync(buffer, 0, buffer.Length);
        Task.WaitAll();
        
        output.Close();
        response.Close();
        
        Listen();
    }

    private byte[] GetFile(string rawUrl)
    {
        var buffer = Array.Empty<byte>();
        var filePath = _serverSettings.Path + rawUrl;
        
        if (Directory.Exists(filePath))
        {
            filePath += "/index.html";
            if (File.Exists(filePath))
                buffer = File.ReadAllBytes(filePath);
        }
        else if (File.Exists(filePath))
            buffer = File.ReadAllBytes(filePath);

        return buffer;
    }

    public void Dispose() => Stop();
}