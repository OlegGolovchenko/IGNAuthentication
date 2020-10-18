using IGNAuthentication.Domain.Interfaces;
using MySql.Data.MySqlClient;
using System.Data.Common;
using IGNAuthentication.Data.Enums;
using IGNAuthentication.Domain.Interfaces.QueryProvider;
using System;
using Microsoft.Extensions.Logging;

namespace IGNAuthentication.Data
{
    public class MySqlDataProvider : IDataProvider
    {
        private readonly string _connectionString;
        private MySqlConnection _connection;
        private ILogger _logger;

        public bool queryToOutput = false;

        public MySqlDataProvider(ILogger logger)
        {
            _logger = logger;
            _connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING"); 
        }

        public void ExecuteNonQuery(IQueryResult query)
        {
            ResetConnection();
            if (_connection == null || _connection.State != System.Data.ConnectionState.Open)
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
            ResetConnection();
            if (_connection == null || _connection.State != System.Data.ConnectionState.Open)
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

        public void ResetConnection()
        {
            try
            {
                _connection.Close();
                _connection.Dispose();
            }
            catch (Exception e)
            {

            }
            _connection = null;
        }

        public void Dispose()
        {
            try
            {
                _connection.Close();
            }
            catch (Exception e)
            {

            }
            finally
            {

                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
