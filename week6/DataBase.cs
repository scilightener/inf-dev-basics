using System.Data.SqlClient;

namespace HttpServer;
internal class DataBase
{
    private readonly string _connectionString;

    public DataBase(string catalog)
        => _connectionString = @$"Data Source=DESKTOP-MFCEQVI\SQLEXPRESS;Initial Catalog={catalog};Integrated Security=True";

    public IEnumerable<T> Select<T>(string query) where T : class
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var cmd = new SqlCommand(query, connection);
        using var reader = cmd.ExecuteReader();

        var result = new List<T>();
        if (!reader.HasRows || !reader.Read()) return result;

        var ctor = typeof(T).GetConstructor(
            Enumerable.Range(0, reader.FieldCount)
            .Select(reader.GetFieldType)
            .ToArray());

        if (ctor is null) return result;

        var parameters = Enumerable.Range(0, reader.FieldCount)
            .Select(reader.GetValue)
            .ToArray();

        result.Add((T)ctor.Invoke(parameters));

        while (reader.Read())
        {
            parameters = Enumerable.Range(0, reader.FieldCount)
                .Select(reader.GetValue)
                .ToArray();

            result.Add((T)ctor.Invoke(parameters));
        }

        return result;
    }

    public void Insert(string query)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var cmd = new SqlCommand(query, connection);
        cmd.ExecuteNonQuery();
    }
}