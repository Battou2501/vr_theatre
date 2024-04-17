using System;
using System.Collections.Generic;
using UnityEngine;

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
        
        public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, Vector3 linePoint1A, Vector3 linePoint1B, Vector3 linePoint2, Vector3 lineVec2){

            closestPointLine1 = Vector3.zero;
            //closestPointLine2 = Vector3.zero;

            var lineVec1 = linePoint1B - linePoint1A;
            
            var a = Vector3.Dot(lineVec1, lineVec1);
            var b = Vector3.Dot(lineVec1, lineVec2);
            var e = Vector3.Dot(lineVec2, lineVec2);

            var d = a*e - b*b;

            //lines are not parallel
            if (d == 0.0f) return false;
            
            Vector3 r = linePoint1A - linePoint2;
            var c = Vector3.Dot(lineVec1, r);
            var f = Vector3.Dot(lineVec2, r);

            var s = (b*f - c*e) / d;
            //var t = (a*f - c*b) / d;

            closestPointLine1 = linePoint1A + lineVec1 * s;
            //closestPointLine2 = linePoint2 + lineVec2 * t;

            return true;

        }
    }
}