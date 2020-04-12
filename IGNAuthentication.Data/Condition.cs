using IGNAuthentication.Domain.Interfaces;

namespace IGNAuthentication.Data
{
    internal class Condition : ICondition
    {
        private string _query;

        public Condition(string query)
        {
            _query = query;
        }

        public ICondition And()
        {
            _query += "AND ";
            return new Condition(_query);
        }

        public ICondition BoolEqual(bool value)
        {
            _query += $"= {(value ? 1 : 0)} ";
            return new Condition(_query);
        }

        public ICondition BoolNotEqual(bool value)
        {
            _query += $"!= {(value ? 1 : 0)} ";
            return new Condition(_query);
        }

        public ICondition Field(string name)
        {
            _query += $"{name} ";
            return new Condition(_query);
        }

        public IQueryResult Go()
        {
            _query += ";";
            return new QueryResult(_query);
        }

        public ICondition LongEqual(long value)
        {
            _query += $"= {value} ";
            return new Condition(_query);
        }

        public ICondition LongNotEqual(long value)
        {
            _query += $"!= {value} ";
            return new Condition(_query);
        }

        public ICondition Or()
        {
            _query += "OR ";
            return new Condition(_query);
        }

        public ICondition StringEqual(string value)
        {
            _query += $"= '{value}' ";
            return new Condition(_query);
        }

        public ICondition StringNotEqual(string value)
        {
            _query += $"!= '{value}' ";
            return new Condition(_query);
        }
    }
}