using System;
using System.Linq;
using System.Reflection;
using StructureMap.Pipeline;

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

        public static Func<Instance, bool> DoesNotHaveAttribute(Type attr)
        {
            return instance => !ContainsAttribute(attr, instance);
        }

        public static Func<Instance, bool> HasAttribute(Type attr)
        {
            return instance => ContainsAttribute(attr, instance);
        }

        public static bool ContainsAttribute(Type attr, Instance instance)
        {
            var type = instance.ReturnedType ?? instance.GetType();
            return type.GetCustomAttribute(attr, false) != null;
        }
    }
}