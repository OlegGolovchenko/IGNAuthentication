using IGNAuthentication.Domain.Interfaces.QueryProvider;

namespace IGNAuthentication.Data
{
    internal class UpdateQuery : IUpdateQuery
    {
        private string _query;

        public UpdateQuery(string query)
        {
            _query = query;
        }

        public string GetResultingString()
        {
            return _query;
        }

        public IQueryResult SetBooleanFieldFalse(string field)
        {
            _query += $"SET {field} = 0;";
            return new UpdateQuery(_query);
        }

        public IConditionalQuery SetBooleanFieldFalseWithCondition(string field)
        {
            _query += $"SET {field} = 0 ";
            return new ConditionalQuery(_query);
        }

        public IQueryResult SetBooleanFieldTrue(string field)
        {
            _query += $"SET {field} = 1;";
            return new UpdateQuery(_query);
        }

        public IConditionalQuery SetBooleanFieldTrueWithCondition(string field)
        {
            _query += $"SET {field} = 1 ";
            return new ConditionalQuery(_query);
        }

        public IQueryResult SetLongField(string field, long value)
        {
            _query += $"SET {field} = {value};";
            return new UpdateQuery(_query);
        }

        public IConditionalQuery SetLongFieldWithCondition(string field, long value)
        {
            _query += $"SET {field} = {value} ";
            return new ConditionalQuery(_query);
        }

        public IQueryResult SetStringField(string field, string value)
        {
            _query += $"SET {field} = '{value}';";
            return new UpdateQuery(_query);
        }

        public IConditionalQuery SetStringFieldWithCondition(string field, string value)
        {
            _query += $"SET {field} = '{value}' ";
            return new ConditionalQuery(_query);
        }

        public IUpdateQuery Table(string table)
        {
            _query += $"UPDATE {table} ";
            return new UpdateQuery(_query);
        }
    }
}