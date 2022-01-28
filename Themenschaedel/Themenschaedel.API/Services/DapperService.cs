using System.Data;
using Npgsql;

namespace Themenschaedel.API.Services
{
    public class DapperService
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public DapperService(IConfiguration configuration)
        {
            _configuration = configuration;
            string connectionStringRaw = _configuration.GetConnectionString("LocalSqlConnection");
            string connectionStringUser = _configuration["Database:LocalUser"];
            string connectionStringPassword = _configuration["Database:LocalPassword"];
            _connectionString = $"{connectionStringUser}{connectionStringPassword}{connectionStringRaw}";
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }
        public IDbConnection CreateConnection()
            => new NpgsqlConnection(_connectionString);
    }
}
