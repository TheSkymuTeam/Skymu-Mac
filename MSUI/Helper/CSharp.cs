using System;

namespace MSUI.Helper
{
    public static class CSharp
    {
        public static T[] Slice<T>(this T[] arr, int start, int end)
        {
            if (start > end)
            {
                throw new ArgumentException("Start larger than end, that's illegal");
            }
            var dest = new T[end - start + 1];
            Array.Copy(arr, start, dest, 0, dest.Length);
            return dest;
        }

        public static T Do<T>(this T obj, Action<T> act)
        {
            act.Invoke(obj);
            return obj;
        }
    }
}