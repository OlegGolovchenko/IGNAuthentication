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
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public DbDataReader ExecuteReader(string query, out DbConnection connection)
        {
            connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand(query, (SqlConnection)connection);
            DbDataReader result = command.ExecuteReader();
            return result;
        }
    }
}
