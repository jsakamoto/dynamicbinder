using System;
using System.Collections.Generic;
using System.Reflection;

namespace Toolbelt.DynamicBinderExtension
{
    public static class DynamicBinderExtension
    {
        public static LateBinder ToLateBind(this object self)
        {
            return new LateBinder(self);
        }

        public static LateBinder ToLateBind(this object self, IDictionary<Type, IDictionary<string, MemberInfo>> cache)
        {
            return new LateBinder(self).SetCache(cache);
        }

        public static dynamic ToDynamic(this object self)
        {
            return new DynamicBinder(new LateBinder(self));
        }

        public static dynamic ToDynamic(this object self, IDictionary<Type, IDictionary<string, MemberInfo>> cache)
        {
            return new DynamicBinder(new LateBinder(self).SetCache(cache));
        }
    }
}
