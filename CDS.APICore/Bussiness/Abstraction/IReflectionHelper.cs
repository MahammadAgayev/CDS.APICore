using System.Collections.Generic;

namespace CDS.APICore.Bussiness.Abstraction
{
    public interface IReflectionHelper
    {
        Dictionary<string, object> GetKeyValue(object o);
    }
}
