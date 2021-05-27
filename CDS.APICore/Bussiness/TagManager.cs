
using System;
using System.Collections.Generic;
using System.Text;

using CDS.APICore.Bussiness.Abstraction;

namespace CDS.APICore.Bussiness
{
    public class TagManager : ITagManager
    {
        public string Tag(Dictionary<string, string> valuePairs)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var val in valuePairs)
            {
                sb.Append($"{val.Key}_{val.Value}_");
            }

            return sb.ToString();
        }

        public Dictionary<string, string> UnTag(string tag)
        {
            var parts = tag.Split('_');

            var dict = new Dictionary<string, string>();

            for (int i = 1; i < parts.Length; i+=2)
            {
                dict.Add(parts[i - 1], parts[i]);
            }

            return dict;
        }

        public T UnTag<T>(string tag, string key)
        {
            var values = this.UnTag(tag);

            return (T)Convert.ChangeType(values[key], typeof(T));
        }
    }
}
