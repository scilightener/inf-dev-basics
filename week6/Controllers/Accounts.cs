using HttpServer.Attributes;
using HttpServer.Models;

namespace HttpServer.Controllers;

[HttpController("accounts")]
public class AccountController
{
    private const string _dbName = "SteamDB";
    private const string _tableName = "[dbo].[Accounts]";

    [HttpGET]
    public static List<Account> GetAccounts()
    {
        var db = new DataBase(_dbName);
        var query = $"select * from {_tableName}";
        return db.Select<Account>(query).ToList();
    }

    [HttpGET(@"\d")]
    public static Account? GetAccountById(int id)
    {
        var db = new DataBase(_dbName);
        var query = $"select * from {_tableName} where id = {id}";
        return db.Select<Account>(query).FirstOrDefault();
    }

    [HttpPOST]
    public static void SaveAccount(string login, string password)
    {
        var db = new DataBase(_dbName);
        var query = $"insert into {_tableName} (login, password) values ('{login}', '{password}')";
        db.Insert(query);
    }
}