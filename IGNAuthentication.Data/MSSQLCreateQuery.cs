using IGNAuthentication.Domain.DataRelated;
using IGNAuthentication.Domain.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace IGNAuthentication.Data
{
    internal class MSSQLCreateQuery : ICreateQuery
    {
        private string _query;

        public MSSQLCreateQuery(string query)
        {
            _query = query;
        }

        public string GetResultingString()
        {
            return _query;
        }

        public IQueryResult TableIfNotExists(string name, IEnumerable<TableField> fields)
        {
            var query = new StringBuilder();
            query.Append("IF NOT EXISTS(SELECT * FROM sysobjects WHERE name='");
            query.Append(name);
            query.Append("' and xtype='U')");
            query.AppendLine();
            query.Append("CREATE TABLE ");
            query.Append(name);
            query.Append("(");
            foreach (var col in fields)
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

            _query += query;

            return new QueryResult(_query);
        }
    }
}
