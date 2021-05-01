
using System.Collections.Generic;

using CDS.APICore.Entities;

namespace CDS.APICore.Bussiness.Abstraction
{
    public interface ICateringManager
    {
        void Create(Catering catering);
        void CreateCategory(CateringCategory category);

        List<Catering> Get();
    }
}
