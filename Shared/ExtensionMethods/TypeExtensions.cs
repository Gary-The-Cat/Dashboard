using System;
using System.Linq;

namespace Shared.ExtensionMethods
{
    public static class TypeExtensions
    {
        public static bool Implements(this Type type, Type interfaceType)
        {
            if (type.GetInterfaces().Any(i => i == interfaceType))
            {
                return true;
            }

            return false;
        }
    }
}
