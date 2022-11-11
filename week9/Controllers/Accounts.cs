using HttpServer.Attributes;
using HttpServer.Models;
using HttpServer.ORM;

namespace HttpServer.Controllers;

[HttpController("accounts")]
public class AccountController
{
    private const string DbName = "SteamDB";
    private const string TableName = "[dbo].[Accounts]";

    /// <summary>
    /// Finds userId in database with given login and password
    /// </summary>
    /// <returns>accountId with login=login and password=password. If not any, returns -1 as not found</returns>
    [HttpPOST]
    public static int Login(string login, string password)
    {
        var dao = new AccountDao();
        var account = dao.GetAll().Where(acc => acc.Login == login && acc.Password == password);
        return account.Any() ? account.First().Id : -1;
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