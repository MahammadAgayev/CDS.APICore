using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.DataAccess.Abstraction;
using CDS.APICore.Helpers;

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

        public string GetValue(string key)
        {
            return _db.ExecuteScalar<string>($"select [value] from {_table} where [key] = @key", null, new Dictionary<string, object>
            {
                { "key", key }
            });
        }

        public void SetValue(string key, string value)
        {
            int affectedRow = _db.ExecuteScalar<int>($"update {_table} set value = @value where [key] = @key", null, new Dictionary<string, object>
            {
                 { "value", value },
                 { "key", key }
            });

            if (affectedRow == 0)
            {
                try
                {
                    _ = _db.Insert(_table, null, new Dictionary<string, object>
                    {
                         { "value", value },
                         { "key", key }
                    });
                }
                catch
                {
                    _db.ExecuteScalar<int>($"update {_table} set value = @value where [key] = @key", null, new Dictionary<string, object>
                    {
                         { "value", value },
                         { "key", key }
                    });
                }
            }
        }
    }
}
