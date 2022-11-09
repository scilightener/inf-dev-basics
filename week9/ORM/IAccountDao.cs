using HttpServer.Models;

namespace HttpServer.ORM;

public interface IAccountDao
{
    public IEnumerable<Account> GetAll();
    public Account? GetById(int id);
    public void Insert(string login, string password);
    public void Remove(int? id);
    public void Update(string field, string value, int? id);
}