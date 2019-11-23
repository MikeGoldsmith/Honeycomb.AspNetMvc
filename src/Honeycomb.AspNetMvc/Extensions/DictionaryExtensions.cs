using System.Collections.Generic;

namespace Honeycomb.AspNetMvc.Extensions
{
    public static class DictionaryExtensions
    {
        public static void TryAdd<T, T1>(this Dictionary<T, T1> dictionary, T key, T1 value)
        {
            if (dictionary.ContainsKey(key))
                return;

            dictionary.Add(key, value);
        }
    }
}