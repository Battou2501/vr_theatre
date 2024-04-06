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
        
        public static void for_each<T, C, D>(this IEnumerable<T> arr, C data1, D data2, Action<T, C, D> action)
        {
            foreach (var a in arr)
            {
                action(a, data1, data2);
            }
        }
        
        public static void for_each<T, C, D, E>(this IEnumerable<T> arr, C data1, D data2, E data3, Action<T, C, D, E> action)
        {
            foreach (var a in arr)
            {
                action(a, data1, data2, data3);
            }
        }
    }
}