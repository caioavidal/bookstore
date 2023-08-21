using System.Data;
using Npgsql;

namespace BookStore.Infra.Data;

public class DbConnection : IDbConnection
{
    private readonly NpgsqlConnection _connection;

    public DbConnection(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString);
        _connection.Open();
    }

    public string ConnectionString
    {
        get => _connection.ConnectionString;
        set => _connection.ConnectionString = value;
    }

    public int ConnectionTimeout => _connection.ConnectionTimeout;

    public string Database => _connection.Database;

    public ConnectionState State => _connection.State;

    public IDbTransaction BeginTransaction()
    {
        return _connection.BeginTransaction();
    }

    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
        return _connection.BeginTransaction(il);
    }

    public void ChangeDatabase(string databaseName)
    {
        _connection.ChangeDatabase(databaseName);
    }

    public void Close()
    {
        _connection.Close();
    }

    public IDbCommand CreateCommand()
    {
        return _connection.CreateCommand();
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    public void Open()
    {
        if (_connection.State == ConnectionState.Open) return;
        _connection.Open();
    }
}