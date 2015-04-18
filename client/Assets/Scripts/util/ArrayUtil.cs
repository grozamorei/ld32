using System;

namespace util
{
    public class ArrayUtil
    {
        public static T[] sumArrays<T> (T[] a1, T[] a2)
        {
            T[] array_sum = new T[a1.Length + a2.Length];
            Array.Copy(a1, array_sum, a1.Length);
            Array.Copy(a2, array_sum, a2.Length);
            return array_sum;
        }
    }
}
