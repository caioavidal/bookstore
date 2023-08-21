using System.Data;

namespace BookStore.Infra.Data.Repositories.Contracts;

public interface IDapperWrapper
{
    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql,
        Func<TFirst, TSecond, TReturn> map,
        object param = null, IDbTransaction transaction = null, bool buffered = true,
        string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

    Task<IEnumerable<TReturn>> QueryAsync<TReturn>(string sql,
        object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null,
        CommandType? commandType = null);

    Task<int> ExecuteAsync(string sql, object param = null,
        IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);

    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
        object param = null, IDbTransaction transaction = null, bool buffered = true,
        string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null);

    Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null,
        IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
}