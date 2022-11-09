using System.Data.SqlClient;
using HttpServer.Models;

namespace HttpServer.ORM;

public class AccountRepository : IAccountRepository
{
    private const string ConnectionString =
        $@"Data Source=DESKTOP-MFCEQVI\SQLEXPRESS;Initial Catalog={DbName};Integrated Security=True";

    private const string TableName = "[dbo].[Accounts]";
    private const string DbName = "SteamDB";
    private readonly Dictionary<int, Account> _accounts;

    public AccountRepository()
    {
        _accounts = new Dictionary<int, Account>();
        foreach (var account in GetAll())
            _accounts.Add(account.Id, account);
    }

    public IEnumerable<Account> GetAll()
    {
        if (!_accounts.Any())
        {
            var query = $"select * from {TableName}";
            using var connection = new SqlConnection(ConnectionString);
            connection.Open();

            var cmd = new SqlCommand(query, connection);
            using var reader = cmd.ExecuteReader();
            
            if (!reader.HasRows) yield break;
            while (reader.Read())
                yield return new Account(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2));
        }
        else
            foreach (var account in _accounts.Values)
                yield return account;
    }

    public Account? GetById(int id) => _accounts.GetValueOrDefault(id);

    public void Insert(Account account)
    {
        var query = $"insert into {TableName} values ('{account.Login}', '{account.Password}')";
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        var cmd = new SqlCommand(query, connection);
        cmd.ExecuteNonQuery();
        _accounts.Add(account.Id, account);
    }

    public void Remove(Account account)
    {
        var query = $"delete from {TableName} where Id={account.Id}";
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        var cmd = new SqlCommand(query);
        cmd.ExecuteNonQuery();
        _accounts.Remove(account.Id);
    }

    public void Update(Account old, Account @new)
    {
        var query = $"update {TableName} set Login={@new.Login}, Password={@new.Password} where Id={old.Id}";

        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        var cmd = new SqlCommand(query);
        cmd.ExecuteNonQuery();
        _accounts[old.Id] = @new;
    }
}