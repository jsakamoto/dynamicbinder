using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Toolbelt.DynamicBinderExtension;

namespace Toolbelt
{
    public class LateBinder
    {
        protected object _Target;

        protected Type _TypeOfTarget;

        protected BindingFlags _BindingFlags;

        public static LateBinder CreateInstance<T>(params object[] args)
        {
            return CreateInstance(typeof(T), args);
        }

        public static LateBinder CreateInstance(Type type, params object[] args)
        {
            Binder.UnwrapBinder(args);
            var obj = Activator.CreateInstance(type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, args, default(CultureInfo));
            return obj.ToLateBind();
        }

        protected IDictionary<Type, IDictionary<string, MemberInfo>> _Cache;

        public PropertyBinder Prop { get; protected set; }

        public FieldBinder Field { get; protected set; }

        public LateBinder(object target)
        {
            this.Initi(target, target.GetType());
        }

        protected LateBinder(object target, Type typeOfTarget)
        {
            this.Initi(target, typeOfTarget);
        }

        private void Initi(object target, Type typeOfTarget)
        {
            this._Target = target;
            this._TypeOfTarget = typeOfTarget;
            this._BindingFlags = BindingFlags.Public | BindingFlags.NonPublic | (target != null ? BindingFlags.Instance : BindingFlags.Static);
            this.Prop = new PropertyBinder(this);
            this.Field = new FieldBinder(this);
        }

        public static LateBinder Create(Type type)
        {
            return new LateBinder(null, type);
        }

        public static LateBinder Create<T>()
        {
            return new LateBinder(null, typeof(T));
        }

        public LateBinder SetCache(IDictionary<Type, IDictionary<string, MemberInfo>> cache)
        {
            this._Cache = cache;
            return this;
        }

        /// <summary>get the object that late binding taret.</summary>
        public object Object { get { return this._Target; } }

        public object Call(string methodName, params object[] args)
        {
            Binder.UnwrapBinder(args);
            var argTypes = args.Select(_ => _ != null ? _.GetType() : typeof(object)).ToArray();
            var memberSufix = "(" + string.Join(",", argTypes.Select(t => t.FullName)) + ")";
            var methodInfo = this.FindMember(
                methodName,
                finder: t =>
                {
                    var method = t.GetMethod(methodName, this._BindingFlags, null, argTypes, null);
                    if (method != null) return method;
                    var methods = t.GetMethods(this._BindingFlags).Where(m => m.Name == methodName).ToArray();
                    if (methods.Length == 1) return methods.First();
                    return methods.Where(m =>
                    {
                        var parameters = m.GetParameters();
                        return parameters
                            .Select((p, index) => p.ParameterType.FullName.TrimEnd('&') == argTypes[index].FullName)
                            .All(_ => _);
                    }).FirstOrDefault();
                },
                memberSufix: memberSufix);
            return methodInfo.Invoke(this._Target, args);
        }

        protected static IEnumerable<Type> EnumType(Type type)
        {
            for (; type != null; type = type.BaseType)
            {
                yield return type;
            }
        }

        protected IDictionary<string, MemberInfo> GetCacheOfMe()
        {
            if (this._Cache == null) return null;
            var cacheOfMe = default(IDictionary<string, MemberInfo>);
            lock (this._Cache)
            {
                if (this._Cache.TryGetValue(this._TypeOfTarget, out cacheOfMe) == false)
                {
                    cacheOfMe = new Dictionary<string, MemberInfo>();
                    this._Cache.Add(this._TypeOfTarget, cacheOfMe);
                }
            }
            return cacheOfMe;
        }

        internal T FindMember<T>(
            string memberName,
            Func<Type, T> finder,
            bool throwExceptionIfMemberNotFound = true,
            string memberSufix = ""
            ) where T : MemberInfo
        {
            Func<T> findMember = () =>
            {
                var memberInfo = EnumType(this._TypeOfTarget)
                    .Select(t => finder(t))
                    .FirstOrDefault(m => m != null);
                if (memberInfo == null && throwExceptionIfMemberNotFound)
                    throw new Exception("Member " + memberName + memberSufix + " not found.");
                return memberInfo;
            };

            var cacheOfMe = this.GetCacheOfMe();
            if (cacheOfMe == null)
            {
                return findMember();
            }
            else
            {
                lock (cacheOfMe)
                {
                    var memberInfo = default(MemberInfo);
                    if (cacheOfMe.TryGetValue(memberName + memberSufix, out memberInfo) == false)
                    {
                        memberInfo = findMember();
                        cacheOfMe.Add(memberName + memberSufix, memberInfo);
                    }
                    return (T)memberInfo;
                }
            }
        }

        public class PropertyBinder
        {
            protected LateBinder _Binder;

            internal PropertyBinder(LateBinder accessor)
            {
                this._Binder = accessor;
            }

            public PropertyInfo GetInfo(string propName, bool throwExceptionIfMemberNotFound = true)
            {
                return this._Binder.FindMember(propName, t => t.GetProperty(propName, this._Binder._BindingFlags), throwExceptionIfMemberNotFound);
            }

            public object this[string propName]
            {
                get
                {
                    var propInfo = this.GetInfo(propName);
                    return propInfo.GetValue(this._Binder._Target, null);
                }
                set
                {
                    var propInfo = this.GetInfo(propName);
                    propInfo.SetValue(this._Binder._Target, value, null);
                }
            }

            public bool Has(string propName)
            {
                return this.GetInfo(propName, false) != null;
            }
        }

        public class FieldBinder
        {
            protected LateBinder _Binder;

            internal FieldBinder(LateBinder accessor)
            {
                this._Binder = accessor;
            }

            public FieldInfo GetInfo(string fieldName, bool throwExceptionIfMemberNotFound = true)
            {
                return this._Binder.FindMember(fieldName, t => t.GetField(fieldName, this._Binder._BindingFlags), throwExceptionIfMemberNotFound);
            }

            public object this[string fieldName]
            {
                get
                {
                    var fieldInfo = this.GetInfo(fieldName);
                    return fieldInfo.GetValue(this._Binder._Target);
                }
                set
                {
                    var fieldInfo = this.GetInfo(fieldName);
                    fieldInfo.SetValue(this._Binder._Target, value);
                }
            }

            public bool Has(string fieldName)
            {
                return this.GetInfo(fieldName, false) != null;
            }
        }
    }
}
