using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Dapper;
using Npgsql;
using Themenschaedel.Shared.Models;

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

            // Add support for custom column attribute for User Property Class
            Dapper.SqlMapper.SetTypeMap(
                typeof(User),
                new CustomPropertyTypeMap(
                    typeof(User),
                    (type, columnName) =>
                        type.GetProperties().FirstOrDefault(prop =>
                            prop.GetCustomAttributes(false)
                                .OfType<ColumnAttribute>()
                                .Any(attr => attr.Name == columnName))));

            // Add support for custom column attribute for Topic Property Class
            Dapper.SqlMapper.SetTypeMap(
                typeof(Topic),
                new CustomPropertyTypeMap(
                    typeof(Topic),
                    (type, columnName) =>
                        type.GetProperties().FirstOrDefault(prop =>
                            prop.GetCustomAttributes(false)
                                .OfType<ColumnAttribute>()
                                .Any(attr => attr.Name == columnName))));

            // Add support for custom column attribute for Subtopic Property Class
            Dapper.SqlMapper.SetTypeMap(
                typeof(Subtopic),
                new CustomPropertyTypeMap(
                    typeof(Subtopic),
                    (type, columnName) =>
                        type.GetProperties().FirstOrDefault(prop =>
                            prop.GetCustomAttributes(false)
                                .OfType<ColumnAttribute>()
                                .Any(attr => attr.Name == columnName))));

            // Add support for custom column attribute for Claim Property Class
            Dapper.SqlMapper.SetTypeMap(
                typeof(Claim),
                new CustomPropertyTypeMap(
                    typeof(Claim),
                    (type, columnName) =>
                        type.GetProperties().FirstOrDefault(prop =>
                            prop.GetCustomAttributes(false)
                                .OfType<ColumnAttribute>()
                                .Any(attr => attr.Name == columnName))));
        }
        public IDbConnection CreateConnection()
            => new NpgsqlConnection(_connectionString);
    }
}
