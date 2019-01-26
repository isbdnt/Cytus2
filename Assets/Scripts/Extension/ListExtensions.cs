using System.Collections.Generic;

namespace Cytus2
{
    public static class ListExtensions
    {
        public static void RemoveAtSwapBack<T>(this List<T> list, int index)
        {
            int last = list.Count - 1;
            list[index] = list[last];
            list.RemoveAt(last);
        }
    }
}