using System.Collections.Generic;
using System.Linq;

using CDS.APICore.Bussiness.Abstraction;

namespace CDS.APICore.Bussiness
{
    public class ReflectionHelper : IReflectionHelper
    {
        public Dictionary<string, object> GetKeyValue(object o)
        {
            return o.GetType().GetProperties().Where(x => x.GetValue(o) != default).ToDictionary(x => x.Name, x => x.GetValue(o));
        }
    }
}