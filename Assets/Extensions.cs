using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    public static class Extensions
    {
        public static void for_each<T>(this IEnumerable<T> arr, Action<T> action)
        {
            foreach (var a in arr)
            {
                action(a);
            }
        }
        
        public static void for_each<T, D>(this IEnumerable<T> arr, D data, Action<T, D> action)
        {
            foreach (var a in arr)
            {
                action(a, data);
            }
        }
    }
}