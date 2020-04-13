using IGNAuthentication.Data.Enums;
using IGNAuthentication.Domain.Interfaces;
using IGNAuthentication.Domain.Interfaces.QueryProvider;
using System.Data.Common;
using System.Data.SqlClient;

namespace IGNAuthentication.Data
{
    public class MsSqlDataProvider : IDataProvider
    {
        private readonly string _connectionString;
        private SqlConnection _connection;

        public MsSqlDataProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ExecuteNonQuery(IQueryResult query)
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }
            var command = new SqlCommand(query.GetResultingString(), _connection);
            command.ExecuteNonQuery();
        }

        public DbDataReader ExecuteReader(IQueryResult query)
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }
            var command = new SqlCommand(query.GetResultingString(), _connection);
            DbDataReader result = command.ExecuteReader();
            return result;
        }

        public IQuery Query()
        {
            return new SqlQuery() { Dialect = DialectEnum.MSSQL };
        }
    }
}
