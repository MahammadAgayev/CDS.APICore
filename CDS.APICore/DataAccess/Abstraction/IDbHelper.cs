using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CDS.APICore.DataAccess.Abstraction
{
    public interface IDbHelper : IDisposable
    {
        IDbTransaction CreateTransaction();

        string[] GetColumns(Type t) => t.GetProperties().Select(x => x.Name).ToArray();

        DataRow Insert(string tablename, IDbTransaction transaction, Dictionary<string, object> parametres);
        int Update(string tablename, IDbTransaction transaction, Dictionary<string, object> parametres, params Filter[] filters);

        DataTable SimpleGet(string tablename, string[] columns, IDbTransaction transaction, Dictionary<string, object> parametres, params Filter[] filters);
        DataTable JoinedGet(string tablename, string[] columns, Join[] joins, IDbTransaction transaction, Dictionary<string, object> parametres, params Filter[] filters);


        DataTable SimpleGet(string tablename, string[] columns, Dictionary<string, object> parametres, params Filter[] filters) => SimpleGet(tablename, columns, null, parametres, filters);
        DataTable JoinedGet(string tablename, string[] columns, Join[] joins, Dictionary<string, object> parametres, params Filter[] filters) => JoinedGet(tablename, columns, joins, null, parametres, filters);

        DataTable Query(string query, IDbTransaction tx, Dictionary<string, object> parametres);
        T ExecuteScalar<T>(string query, IDbTransaction tx, Dictionary<string, object> parametres);
        void Execute(string query, IDbTransaction tx, Dictionary<string, object> parametres);
    }
}
