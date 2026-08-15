// ReSharper disable once CheckNamespace

using System;
using System.Diagnostics;

namespace MSUI.Helper.Log
{
    public static class Logger
    {
        public static T ConsoleLog<T>(this T obj, string fmt, params object[] shit)
        {
            Console.WriteLine($"[{obj.GetType().Name}] {fmt}", shit);
            return obj;
        }
        public static T DebugLog<T>(this T obj, string fmt, params object[] shit)
        {
            Debug.WriteLine($"[{obj.GetType().Name}] {fmt}", shit);
            return obj;
        }
    }
}