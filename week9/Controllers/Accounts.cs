using HttpServer.Attributes;
using HttpServer.Models;
using HttpServer.ORM;
using HttpServer.ServerLogic.SessionLogic;

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
        var account = dao.GetAll().FirstOrDefault(acc => acc.Login == login && acc.Password == password);
        if (account is null)
            return -1;
        SessionManager.CreateSession(account.Id, login, DateTime.Now);
        return account.Id;
    }

    [HttpGET(@"\d")]
    public static Account? GetAccountInfo(int id)
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