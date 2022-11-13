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
    /// <returns>(accountId, sessionId) of user with login=login and password=password. If not any, returns (-1, Guid.Empty) as not found</returns>
    [HttpPOST]
    public static (int, Guid) Login(string login, string password)
    {
        var dao = new AccountDao();
        var account = dao.GetAll().FirstOrDefault(acc => acc.Login == login && acc.Password == password);
        if (account is null)
            return (-1, Guid.Empty);
        var sessionId = SessionManager.CreateSession(account.Id, login, DateTime.Now);
        return (account.Id, sessionId);
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