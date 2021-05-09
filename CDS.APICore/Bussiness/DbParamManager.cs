using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.DataAccess.Abstraction;

namespace CDS.APICore.Bussiness
{
    public class DbParamManager : IParamManager
    {
        private const string _table = "parametres";

        private readonly IDbHelper _db;

        public DbParamManager(IDbHelper db)
        {
            _db = db;
        }

        public T GetValue<T>(string key)
        {
            return _db.ExecuteScalar<T>($"select [value] from {_table} where [key] = @key", null, new Dictionary<string, object>
            {
                { "key", key }
            });
        }

        public void SetValue(string key, object value)
        {
            int affectedRow = _db.Update(_table, null, new Dictionary<string, object>
            {
                 { "key", key }
            }, new DataAccess.Filter { Comparison = DataAccess.Comparison.Equal, Name = "key" });

            if(affectedRow == 0)
            {
                try
                {
                    _ = _db.Insert(_table, null, new Dictionary<string, object>
                    {
                         { "key", key }
                    });
                }
                catch
                {
                    _db.Update(_table, null, new Dictionary<string, object>
                    {
                         { "key", key }
                    }, new DataAccess.Filter { Comparison = DataAccess.Comparison.Equal, Name = "key" });
                }
            }
        }
    }
}
