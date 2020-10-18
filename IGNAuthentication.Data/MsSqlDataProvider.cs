using IGNAuthentication.Data.Enums;
using IGNAuthentication.Domain.Interfaces;
using IGNAuthentication.Domain.Interfaces.QueryProvider;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.IsisMtt.X509;

namespace IGNAuthentication.Data
{
    public class MsSqlDataProvider : IDataProvider
    {
        private readonly string _connectionString;
        private SqlConnection _connection;
        private ILogger _logger;

        public bool queryToOutput = false;

        public MsSqlDataProvider(ILogger logger)
        {
            _logger = logger;
            _connectionString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION_STRING");
        }

        public void ExecuteNonQuery(IQueryResult query)
        {
            ResetConnection();
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }
            if (queryToOutput)
            {
                Console.WriteLine(query.GetResultingString());
            }
            var command = new SqlCommand(query.GetResultingString(), _connection);
            command.ExecuteNonQuery();
        }

        public DbDataReader ExecuteReader(IQueryResult query)
        {
            ResetConnection();
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }
            if (queryToOutput)
            {
                Console.WriteLine(query.GetResultingString());
            }
            var command = new SqlCommand(query.GetResultingString(), _connection);
            DbDataReader result = command.ExecuteReader();
            return result;
        }

        public IQuery Query()
        {
            return new SqlQuery() { Dialect = DialectEnum.MSSQL };
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
