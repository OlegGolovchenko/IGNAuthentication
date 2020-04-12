using IGNAuthentication.Data.Enums;
using IGNAuthentication.Domain.Interfaces;
using System;

namespace IGNAuthentication.Data
{
    internal class SqlQuery : IQuery
    {
        private readonly string _query;

        public DialectEnum Dialect { get; set; }

        public SqlQuery()
        {
            _query = string.Empty;
        }

        public ICreateQuery Create()
        {
            if (Dialect == DialectEnum.MySQL)
            {
                return new CreateQuery(_query);
            }
            else if(Dialect == DialectEnum.MSSQL)
            {
                return new MSSQLCreateQuery(_query);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public IDeleteQuery Delete()
        {
            return new DeleteQuery(_query);
        }

        public string GetResultingString()
        {
            return _query;
        }

        public IInsertQuery Insert()
        {
            return new InsertQuery(_query);
        }

        public ISelectQuery Select()
        {
            return new SelectQuery(_query);
        }

        public IUpdateQuery Update()
        {
            return new UpdateQuery(_query);
        }
    }
}
