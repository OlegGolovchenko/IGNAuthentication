using IGNAuthentication.Domain.DataRelated;
using IGNAuthentication.Domain.Interfaces;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace IGNAuthentication.Data
{
    public class MsSqlDataProvider : IDataProvider
    {
        private string _connectionString;
        private SqlConnection _connection;

        public MsSqlDataProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateTableIfNotExists(string tableName, IEnumerable<TableField> columns)
        {
            var query = new StringBuilder();
            query.Append("IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='");
            query.Append(tableName);
            query.Append("' and xtype='U')");
            query.AppendLine();
            query.Append("CREATE TABLE ");
            query.Append(tableName);
            query.Append("(");
            foreach (var col in columns)
            {
                query.Append(col.Name);
                query.Append(" ");
                query.Append(col.Type);
                query.Append(" ");
                query.Append(col.CanHaveNull ? "null" : "not null");
                if (!string.IsNullOrEmpty(col.DefValue))
                {
                    query.Append(" default ");
                    query.Append(col.DefValue);
                }
                if (col.Generated)
                {
                    query.Append(" IDENTITY(1,1)");
                }
                query.Append(", ");
            }
            query.Append(");");
            ExecuteNonQuery(query.ToString());
        }

        public void ExecuteNonQuery(string query)
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }
            var command = new SqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public DbDataReader ExecuteReader(string query)
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }
            var command = new SqlCommand(query, _connection);
            DbDataReader result = command.ExecuteReader();
            return result;
        }
    }
}
