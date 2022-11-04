using HttpServer.Attributes;
using HttpServer.Models;
using HttpServer.ORM;

namespace HttpServer.Controllers;

[HttpController("accounts")]
public class AccountController
{
    private const string DbName = "SteamDB";
    private const string TableName = "[dbo].[Accounts]";

    [HttpPOST]
    public static void SaveAccount(string login, string password)
    {
        var dao = new AccountDao();
        dao.Insert(login, password);
    }

    [HttpGET(@"\d")]
    public static Account? GetAccountById(int id)
    {
        var dao = new AccountDao();
        return dao.GetById(id);
    }

    [HttpGET]
    public static List<Account> GetAccounts()
    {
        var dao = new AccountDao();
        return dao.GetAll().ToList();
    }
}