using CDS.APICore.Entities;

namespace CDS.APICore.Bussiness.Abstraction
{
    public interface IAccountManager
    {
        void Create(Account account);

        Account Get(string username);

        Account Get(int id);
    }
}
