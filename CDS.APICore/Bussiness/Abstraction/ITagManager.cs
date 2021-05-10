using System.Collections.Generic;

namespace CDS.APICore.Bussiness.Abstraction
{
    public interface ITagManager
    {
        string Tag(Dictionary<string, string> valuePairs);

        Dictionary<string, string> UnTag(string tag);

        T UnTag<T>(string tag, string key);
    }
}
