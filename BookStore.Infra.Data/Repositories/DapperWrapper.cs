using System.Data;
using BookStore.Infra.Data.Repositories.Contracts;
using Dapper;

namespace BookStore.Infra.Data.Repositories;

public class DapperWrapper : IDapperWrapper
{
    private readonly IDbConnection _dbConnection;

    public DapperWrapper(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        object param = null, IDbTransaction transaction = null, bool buffered = true,
        string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
    {
        return _dbConnection.QueryAsync(sql, map, param, transaction,
            buffered, splitOn,
            commandTimeout, commandType);
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql,
        Func<TFirst, TSecond, TReturn> map,
        object param = null, IDbTransaction transaction = null, bool buffered = true,
        string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
    {
        return _dbConnection.QueryAsync(sql, map, param, transaction,
            buffered, splitOn,
            commandTimeout, commandType);
    }

    public Task<IEnumerable<TReturn>> QueryAsync<TReturn>(string sql,
        object param = null, IDbTransaction transaction = null, bool buffered = true,
        int? commandTimeout = null, CommandType? commandType = null)
    {
        return _dbConnection.QueryAsync<TReturn>(sql, param, transaction,
            commandTimeout, commandType);
    }

    public Task<int> ExecuteAsync(string sql, object param = null,
        IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return _dbConnection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
    }

    public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null,
        IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        return _dbConnection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout,
            commandType);
    }
}