namespace HttpServer;

internal static partial class ServerResponseProvider
{
    private static readonly Dictionary<string, string> ContentTypeDictionary = new()
    {
        {"jpeg", "image/jpeg"},
        {"jpg", "image/jpeg"},
        {"png", "image/png"},
        {"bmp", "image/bmp"},
        {"gif", "image/gif"},
        {"webp", "image/webp"},
        {"svg", "image/svg+xml"},
        
        {"mp4", "video/mp4"},
        {"mkv", "video/x-matroska"},
        {"MPV", "video/MPV"},
        {"mpeg", "video/mpeg"},
        {"webm", "video/webm"},
        {"avi", "video/x-msvideo"},
        
        {"mp3", "audio/mpeg"},
        {"aac", "audio/mp4"},
        {"ogg", "audio/ogg"},
        
        {"txt", "text/plain"},
        {"css", "text/css"},
        {"csv", "text/csv"},
        {"html", "text/html"},
        {"php", "text/php"},

        {"json", "application/json"},
        {"pdf", "application/pdf"},
        {"js", "application/javascript"},
        {"xml", "application/xml"},
        {"doc", "application/msword"},
        {"docx", "application/msword"},
        {"tex", "application/x-tex"},
    };
}