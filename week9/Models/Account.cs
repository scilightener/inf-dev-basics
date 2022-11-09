using HttpServer.Attributes;

namespace HttpServer.Models;

public class Account
{
    public int Id { get; }
    [DbItem("login")]
    public string Login { get; }
    [DbItem("password")]
    public string Password { get; }
    
    public Account(int id, string login, string password)
    {
        Id = id;
        Login = login;
        Password = password;
    }

    public Account(string login, string password)
    {
        Login = login;
        Password = password;
    }
}