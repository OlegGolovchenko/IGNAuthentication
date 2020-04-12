using IGNAuthentication.Domain.Interfaces;

namespace IGNAuthentication.Data
{
    internal class QueryResult : IQueryResult
    {
        private readonly string _query;

        public QueryResult(string query)
        {
            _query = query;
        }

        public string GetResultingString()
        {
            return _query;
        }
    }
}
