namespace HttpServer.Attributes;

public class DbItem : Attribute
{
    public readonly string Name;

    public DbItem(string name)
    {
        Name = name;
    }
}