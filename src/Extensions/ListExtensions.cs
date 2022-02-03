using System.Collections.Generic;
using System.Linq;

namespace WebApplication.Extensions
{
    public static class ListExtensions
    {
        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            return list == null || list.FirstOrDefault() == null;
        }
    }
}