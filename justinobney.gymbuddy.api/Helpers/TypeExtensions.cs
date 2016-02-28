using System;
using System.Linq;

namespace justinobney.gymbuddy.api.Helpers
{
    public static class TypeExtensions
    {
        public static string GetPrettyName(this Type type)
        {
            return type.FullName.Split('.').Reverse().First();
        }
    }
}