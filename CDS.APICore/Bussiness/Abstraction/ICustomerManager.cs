
using CDS.APICore.Entities;

namespace CDS.APICore.Bussiness.Abstraction
{
    public interface ICustomerManager
    {
        void Create(Customer customer);
        Customer Get(int id);
        Customer Get(string identitytag);
    }
}