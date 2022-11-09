using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using HttpServer.Attributes;

namespace HttpServer.ORM;

internal class DataBase
{
    private readonly string _connectionString;
    private readonly string _tableName;

    public DataBase(string dbName, string tableName)
    {
        _tableName = tableName;
        _connectionString =
            @$"Data Source=DESKTOP-MFCEQVI\SQLEXPRESS;Initial Catalog={dbName};Integrated Security=True";
    }

    public IEnumerable<T> Select<T>(string? query = null) where T : class
    {
        query ??= $"select * from {_tableName}";
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var cmd = new SqlCommand(query, connection);
        using var reader = cmd.ExecuteReader();

        if (!reader.HasRows || !reader.Read()) yield break;

        var ctor = GetConstructor<T>(reader);
        if (ctor is null) yield break;

        var parameters = new object[reader.FieldCount];
        reader.GetValues(parameters);
        yield return (T)ctor.Invoke(parameters);
        while (reader.Read())
        {
            reader.GetValues(parameters);
            yield return (T)ctor.Invoke(parameters);
        }
    }

    public void Insert<T>(T instance)
    {
        var properties = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute(typeof(DbItem)) is not null)
            .ToDictionary(p => (p.GetCustomAttribute(typeof(DbItem)) as DbItem)!.Name,
                p => $"'{p.GetValue(instance) ?? string.Empty}'");
        var query =
            $"insert into {_tableName} ({string.Join(", ", properties.Keys)}) " +
            $"values {string.Join(", ", properties.Values)}";
        Execute(query);
    }

    public void Delete(int? id = null)
    {
        var query = $"delete from {_tableName}";
        query += id is not null ? $"where Id={id}" : "";
        Execute(query);
    }

    public void Update(string field, string value, int? id = null)
    {
        var query = $"update {_tableName} set {field}={value}";
        query += id is not null ? $"where Id={id}" : "";
        Execute(query);
    }

    private void Execute(string query)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var cmd = new SqlCommand(query, connection);
        cmd.ExecuteNonQuery();
    }

    private static ConstructorInfo? GetConstructor<T>(IDataRecord reader) =>
        typeof(T).GetConstructor(
            Enumerable.Range(0, reader.FieldCount)
                .Select(reader.GetFieldType)
                .ToArray());
}