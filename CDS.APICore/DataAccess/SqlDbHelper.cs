using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CDS.APICore.DataAccess.Abstraction;
using CDS.APICore.Helpers;

namespace CDS.APICore.DataAccess
{
    public class SqlDbHelper : IDbHelper
    {
        private readonly string _connectionString;

        private readonly object _disposeLocker = new object();

        private readonly ConcurrentBag<SqlTransaction> _transactions;

        public SqlDbHelper(string connectionString)
        {
            if (connectionString is null)
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;

            _transactions = new ConcurrentBag<SqlTransaction>();
        }

        public bool Disposed { get; private set; }

        public void Dispose()
        {
            if (this.Disposed)
                throw new ObjectDisposedException(nameof(SqlDbHelper));

            lock (_disposeLocker)
            {
                foreach (var tx in _transactions)
                {
                    var connection = tx.Connection;

                    SafeActionExecutor.Execute(() => tx.Rollback());
                    SafeActionExecutor.Execute(() => tx.Dispose());
                    SafeActionExecutor.Execute(() => tx.Dispose());
                }
            }

            this.Disposed = true;
        }

        public IDbTransaction CreateTransaction() => this.innerCreateTransaction();

        public void Insert(string tablename, IDbTransaction transaction, Dictionary<string, object> parametres)
        {
            string columns = string.Join(',', parametres.Select(x => x.Key));
            string values = string.Join(',', parametres.Select(x => $"@{x.Key}"));

            string query = $"insert into {tablename}({columns}) values ({values})";

            var (tx, con) = this.getConnection(transaction);

            var cmd = con.CreateCommand();
            cmd.CommandText = query;
            cmd.Transaction = tx;

            foreach (var param in parametres)
            {
                _ = cmd.Parameters.AddWithValue(param.Key, param.Value);
            }

            _ = cmd.ExecuteNonQuery();
        }

        public void Update(string tablename, IDbTransaction transaction, Dictionary<string, object> parametres, params Filter[] filters)
        {
            StringBuilder sb = new StringBuilder();

            var setParameters = parametres.Where(a => filters.Any(x => !x.Name.ToLowerInvariant()
            .Contains(a.Key.ToLowerInvariant())));

            string setQuery = string.Join(",", setParameters.Select(x => $"{x.Key} = @{x.Key}"));
            string query = $"update {tablename} set {setQuery} {this.createFilterQuery(filters)}";

            var (tx, con) = this.getConnection(transaction);

            var cmd = con.CreateCommand();
            cmd.CommandText = query;
            cmd.Transaction = tx;

            foreach (var param in parametres)
            {
                _ = cmd.Parameters.AddWithValue(param.Key, param.Value);
            }

            _ = cmd.ExecuteNonQuery();
        }

        public DataTable SimpleGet(string tablename, string[] columns, IDbTransaction transaction,Dictionary<string, object> parametres, params Filter[] filters)
        {
            string columsnQuery = string.Join(",", columns);
            string query = $"select {columsnQuery} from {tablename} {this.createFilterQuery(filters)}";

            return this.executeQuery(query, transaction, parametres);
        }

        public DataTable JoinedGet(string tablename, string[] columns, Join[] joins, IDbTransaction transaction, Dictionary<string, object> parametres, params Filter[] filters)
        {
            var queryColumns = columns.Select(x => $"{tablename}.{x}").ToList();

            var joinColumns = joins.SelectMany(x => x.Columns.Select(y => $"{x.TableName}.{y}"));

            queryColumns.AddRange(joinColumns);

            string allColumns = string.Join(',', queryColumns.ToArray());

            string query = $"select {allColumns} from {tablename}  {this.createJoinQuery(joins)} {this.createFilterQuery(filters)}";

            return this.executeQuery(query, transaction, parametres);
        }


        private SqlTransaction innerCreateTransaction()
        {
            var connection = this.innerCreateConnection();

            var tx = connection.BeginTransaction();

            _transactions.Add(tx);

            return tx;
        }

        private SqlConnection innerCreateConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();

            return connection;
        }

        public (SqlTransaction, SqlConnection) getConnection(IDbTransaction transaction)
        {
            if (transaction is null)
            {
                return (null, this.innerCreateConnection());
            }

            if (!(transaction is SqlTransaction))
            {
                throw new InvalidOperationException("SqlDbHelper only allows sql transactions");
            }

            var tx = transaction as SqlTransaction;
            var con = transaction.Connection as SqlConnection;

            return (tx, con);
        }

        private string createFilterQuery(Filter[] filters)
        {
            StringBuilder sb = new StringBuilder();

            bool isFirst = true;

            foreach (var filter in filters ?? new Filter[0])
            {
                if (isFirst)
                {
                    sb.Append("where ");
                }
                else
                {
                    sb.Append(" and  ");
                }

                string column = filter is JoinFilter ? $"{(filter as JoinFilter).TableName}.{filter.Name}" : filter.Name;

                sb.Append($" {column} {this.getComparisonString(filter.Comparison)} @{filter.Name} ");

                isFirst = false;
            }

            return sb.ToString();
        }

        private string getComparisonString(Comparison comparison)
            => comparison switch
            {
                Comparison.Equal => "=",
                Comparison.Greater => ">",
                Comparison.Lower => "<",
                Comparison.GreaterThan => ">=",
                Comparison.LowerThan => "<=",
                _ => throw new NotSupportedException()
            };

        private DataTable getFromReader(SqlDataReader reader)
        {
            var table = new DataTable();

            table.Load(reader);

            return table;
        }

        public string createJoinQuery(Join[] joins)
        {
            StringBuilder sb = new StringBuilder();

            foreach(var j in joins)
            {
                sb.AppendLine($"{j.JoinType} join {j.TableName} on {j.TableName}.{j.JoinColumn} = {j.JoinsToTableName}.{j.JoinsToColumn}");
            }

            return sb.ToString();
        }

        private DataTable executeQuery(string query, IDbTransaction transaction, Dictionary<string, object> parametres)
        {
            var (tx, con) = this.getConnection(transaction);

            var cmd = con.CreateCommand();
            cmd.CommandText = query;
            cmd.Transaction = tx;

            foreach (var param in parametres ?? new Dictionary<string, object>())
            {
                _ = cmd.Parameters.AddWithValue(param.Key, param.Value);
            }

            var reader = cmd.ExecuteReader();

            return this.getFromReader(reader);
        }
    }
}