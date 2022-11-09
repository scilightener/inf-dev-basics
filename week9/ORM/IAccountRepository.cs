using HttpServer.Models;

namespace HttpServer.ORM;

public interface IAccountRepository
{
    public IEnumerable<Account> GetAll();
    public Account? GetById(int id);
    public void Insert(Account account);
    public void Remove(Account account);
    public void Update(Account old, Account @new);
}