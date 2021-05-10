using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using CDS.APICore.Bussiness.Abstraction;
using CDS.APICore.DataAccess;
using CDS.APICore.DataAccess.Abstraction;
using CDS.APICore.Entities;

namespace CDS.APICore.Bussiness
{
    public class AccountManager : IAccountManager
    {
        private const string _accountTable = "Accounts";

        private readonly IDbHelper _db;

        private readonly IReflectionHelper _reflectionHelper;


        public AccountManager(IDbHelper db, IReflectionHelper reflectionHelper)
        {
            _db = db;
            _reflectionHelper = reflectionHelper;
        }


        public void Create(Account account)
        {
            using var tx = _db.CreateTransaction();

            _db.Insert(_accountTable, tx, new Dictionary<string, object>
                {
                    { nameof(Account.Username), account.Username },
                    { nameof(Account.PasswordHash), account.PasswordHash},
                    { nameof(Account.Created), account.Created },
                    { nameof(Account.Updated), account.Updated }
                });

            tx.Commit();
        }

        public Account Get(string username)
        {
            var data = _db.SimpleGet(_accountTable, _reflectionHelper.GetPropNames(typeof(Account)), new Dictionary<string, object>
            {
                { nameof(Account.Username), username }
            }, new Filter { Comparison = Comparison.Equal, Name = nameof(Account.Username) });

            if (data.Rows.Count == 0)
                return null;

            return this.getFromRow(data.Rows[0]);
        }

        public Account Get(int id)
        {
            var data = _db.SimpleGet(_accountTable, _reflectionHelper.GetPropNames(typeof(Account)), new Dictionary<string, object>
            {
                { nameof(Account.Id), id }
            }, new Filter { Comparison = Comparison.Equal, Name = nameof(Account.Id) });

            if (data.Rows.Count == 0)
                return null;

            return this.getFromRow(data.Rows[0]);
        }

        private Account getFromRow(DataRow dataRow)
        {
            var account = new Account
            {
                Id = Convert.ToInt32(dataRow[nameof(Account.Id)]),
                Username = dataRow[nameof(Account.Username)].ToString(),
                PasswordHash = dataRow[nameof(Account.PasswordHash)].ToString(),
                Created = Convert.ToDateTime(dataRow[nameof(Account.Created)]),
                Updated = Convert.ToDateTime(dataRow[nameof(Account.Updated)]),
            };

            return account;
        }
    }
}
