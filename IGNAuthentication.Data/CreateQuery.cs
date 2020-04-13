using IGNAuthentication.Domain.DataRelated;
using IGNAuthentication.Domain.Interfaces.QueryProvider;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IGNAuthentication.Data
{
    internal class CreateQuery : ICreateQuery
    {
        private string _query;

        public CreateQuery(string query)
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
            query.Append("CREATE TABLE IF NOT EXISTS ");
            query.Append(name);
            query.Append("(");
            var last = fields.LastOrDefault();
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
            var pk = fields.SingleOrDefault(fld => fld.Primary);
            if (pk != null)
            {
                query.Append(", constraint ");
                query.Append("pk_");
                query.Append(name);
                query.Append(pk.Name);
                query.Append(" primary key(");
                query.Append(pk.Name);
                query.Append(")");
            }
            query.Append(");");

            _query += query;

            return new QueryResult(_query);
        }
    }
}
