using CDS.APICore.Entities;

namespace CDS.APICore.Bussiness.Abstraction
{
    public interface IOrderManager
    {
        void Create(Order order);

        void CreateItemCategory(ItemCategory category);

        ItemCategory GetCategory(int id);
    }
}
