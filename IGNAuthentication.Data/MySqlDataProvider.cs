using IGNAuthentication.Domain.Interfaces;
using MySql.Data.MySqlClient;
using System.Data.Common;
using IGNAuthentication.Data.Enums;
using IGNAuthentication.Domain.Interfaces.QueryProvider;
using System;

namespace IGNAuthentication.Data
{
    public class MySqlDataProvider : IDataProvider
    {
        private readonly string _connectionString;
        private MySqlConnection _connection;

        public bool queryToOutput = false;

        public MySqlDataProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ExecuteNonQuery(IQueryResult query)
        {
            if (_connection == null)
            {
                _connection = new MySqlConnection(_connectionString);
                _connection.Open();
            }
            if (queryToOutput)
            {
                Console.WriteLine(query.GetResultingString());
            }
            var command = new MySqlCommand(query.GetResultingString(), _connection);
            command.ExecuteNonQuery();
        }

        public DbDataReader ExecuteReader(IQueryResult query)
        {
            if (_connection == null)
            {
                _connection = new MySqlConnection(_connectionString);
                _connection.Open();
            }
            if (queryToOutput)
            {
                Console.WriteLine(query.GetResultingString());
            }
            var command = new MySqlCommand(query.GetResultingString(), _connection);
            MySqlDataReader result = command.ExecuteReader();
            return result;
        }

        public IQuery Query()
        {
            return new SqlQuery() { Dialect = DialectEnum.MySQL };
        }
    }
}
