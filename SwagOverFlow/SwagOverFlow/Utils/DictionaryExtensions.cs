using System;
using System.Collections.Generic;
using System.Text;

namespace SwagOverFlow.Utils
{
    public static class DictionaryExtensions
    {
        public static void SafeSet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue val)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = val;
            }
            else
            {
                dict.Add(key, val);
            }
        }
    }
}
