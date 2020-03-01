using IGNAuthentication.Domain.DataRelated;
using IGNAuthentication.Domain.Interfaces;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Linq;

namespace IGNAuthentication.Data
{
    public class MySqlDataProvider : IDataProvider
    {
        private string _connectionString;
        private MySqlConnection _connection;

        public MySqlDataProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateTableIfNotExists(string tableName, IEnumerable<TableField> columns)
        {
            var query = new StringBuilder();
            query.Append("CREATE TABLE IF NOT EXISTS ");
            query.Append(tableName);
            query.Append("(");
            var last = columns.LastOrDefault();
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
                    var defValue = col.DefValue;
                    if (col.Type == "bit")
                    {
                        defValue = col.DefValue == "'true'" ? "1" : "0";
                    }
                    query.Append(defValue);
                }
                if (col.Generated)
                {
                    query.Append(" auto_increment");
                }
                if (col != last)
                {
                    query.Append(", ");
                }
            }
            var pk = columns.SingleOrDefault(fld => fld.Primary);
            if (pk != null)
            {
                query.Append(", constraint ");
                query.Append("pk_");
                query.Append(tableName);
                query.Append(pk.Name);
                query.Append(" primary key(");
                query.Append(pk.Name);
                query.Append(")");
            }
            query.Append(");");
            ExecuteNonQuery(query.ToString());
        }

        public void ExecuteNonQuery(string query)
        {
            if (_connection == null)
            {
                _connection = new MySqlConnection(_connectionString);
                _connection.Open();
            }
            var command = new MySqlCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        public DbDataReader ExecuteReader(string query)
        {
            if (_connection == null)
            {
                _connection = new MySqlConnection(_connectionString);
                _connection.Open();
            }
            var command = new MySqlCommand(query, _connection);
            MySqlDataReader result = command.ExecuteReader();
            return result;
        }
    }
}
