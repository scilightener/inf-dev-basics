using System.Text;

namespace HttpServer;

internal static partial class ServerResponseProvider
{
    public static ServerResponse GetResponse(string path, string rawUrl)
    {
        if (!Directory.Exists(path))
            return new ServerResponse(
                Encoding.UTF8.GetBytes($"Directory {path} not found."),
                "text/plain");

        var buffer = GetFile(path + rawUrl.Replace("%20", " "));
        var contentType = GetContentType(rawUrl);
        if (buffer.Length == 0)
        {
            contentType = "text/plain";
            buffer = Encoding.UTF8.GetBytes("Error 404. Not Found.");
        }
        
        return new ServerResponse(buffer, contentType);
    }

    private static byte[] GetFile(string filePath)
    {
        var buffer = Array.Empty<byte>();

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

    private static string GetContentType(string path)
    {
        var ext = path.Contains('.') ? path.Split('.')[^1] : "html";
        return ContentTypeDictionary.ContainsKey(ext) ? ContentTypeDictionary[ext] : "text/plain";
    }
}