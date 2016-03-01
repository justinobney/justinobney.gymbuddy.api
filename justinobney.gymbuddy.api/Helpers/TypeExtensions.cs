using System;
using System.Linq;

namespace justinobney.gymbuddy.api.Helpers
{
    public static class TypeExtensions
    {
        public static string GetPrettyName(this Type type)
        {
            string prettyName = type.Name;
            if (type.IsGenericType)
            {
                int iBacktick = prettyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    prettyName = prettyName.Remove(iBacktick);
                }
                prettyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = GetPrettyName(typeParameters[i]);
                    prettyName += (i == 0 ? typeParamName : "," + typeParamName);
                }
                prettyName += ">";
            }
            else
            {
                prettyName = type.FullName.Split('.').Reverse().First();
            }

            return prettyName.Replace('+', '.');
        }
    }
}