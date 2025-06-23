using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Data;

namespace ServicePortals.Infrastructure.Data
{
    public interface IViclockDapperContext
    {
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text);
        Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text);
        Task<int> ExecuteAsync(string sql, object? parameters = null, CommandType commandType = CommandType.Text);
    }

    public class ViclockDapperContext : IViclockDapperContext
    {
        private readonly string _connectionString;

        public ViclockDapperContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ViclockStringConnectionDb")!;
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                using var connection = CreateConnection();
                return await connection.QueryAsync<T>(sql, parameters, commandType: commandType);
            }
            catch (Exception ex)
            {
                Log.Error($"Error, ex: {ex.Message}");
                throw new Exception($"QueryAsync failed: {ex.Message}", ex);
            }
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<T>(sql, parameters, commandType: commandType);
            }
            catch (Exception ex)
            {
                Log.Error($"Error, ex: {ex.Message}");
                throw new Exception($"QueryAsync failed: {ex.Message}", ex);
            }
        }

        public async Task<int> ExecuteAsync(string sql, object? parameters = null, CommandType commandType = CommandType.Text)
        {
            try
            {
                using var connection = CreateConnection();
                return await connection.ExecuteAsync(sql, parameters, commandType: commandType);
            }
            catch (Exception ex)
            {
                Log.Error($"Error, ex: {ex.Message}");
                throw new Exception($"QueryAsync failed: {ex.Message}", ex);
            }
        }
    }
}

//var sw = Stopwatch.StartNew();
//sw.Stop();
//Console.WriteLine($"Mo ket noi connection > Open took: {sw.ElapsedMilliseconds} ms");