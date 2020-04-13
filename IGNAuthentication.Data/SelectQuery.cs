using IGNAuthentication.Domain.Interfaces.QueryProvider;
using System.Collections.Generic;

namespace IGNAuthentication.Data
{
    internal class SelectQuery : ISelectQuery
    {
        private string _query;

        public SelectQuery(string query)
        {
            _query = query;
        }

        public IQueryResult AllFrom(string table)
        {
            _query += $"SELECT * FROM {table};";
            return new QueryResult(_query);
        }

        public IConditionalQuery AllFromWithCondition(string table)
        {
            _query += $"SELECT * FROM {table} ";
            return new ConditionalQuery(_query);
        }

        public IQueryResult FieldsFrom(string table, IEnumerable<string> fieldNames)
        {
            _query += $"SELECT {string.Join(",", fieldNames)} FROM {table};";
            return new QueryResult(_query);
        }

        public IConditionalQuery FieldsFromWithCondition(string table, IEnumerable<string> fieldNames)
        {
            _query += $"SELECT {string.Join(",", fieldNames)} FROM {table} ";
            return new ConditionalQuery(_query);
        }

        public string GetResultingString()
        {
            return _query;
        }
    }
}