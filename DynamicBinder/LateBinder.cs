using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Toolbelt
{
    public class LateBinder
    {
        protected object _Target;

        protected Type _TypeOfTarget;

        protected BindingFlags _BindingFlags;

        protected IDictionary<Type, IDictionary<string, MemberInfo>> _Cache;

        public PropertyBinder Prop { get; protected set; }

        public FieldBinder Field { get; protected set; }

        public LateBinder(object target)
        {
            Initi(target, target.GetType());
        }

        protected LateBinder(object target, Type typeOfTarget)
        {
            Initi(target, typeOfTarget);
        }

        private void Initi(object target, Type typeOfTarget)
        {
            _Target = target;
            _TypeOfTarget = typeOfTarget;
            _BindingFlags = BindingFlags.Public | BindingFlags.NonPublic | (target != null ? BindingFlags.Instance : BindingFlags.Static);
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
            _Cache = cache;
            return this;
        }

        public object Call(string methodName, params object[] args)
        {
            var argTypes = args.Select(_ => _.GetType()).ToArray();
            var memberSufix = "(" + string.Join(",", argTypes.Select(t => t.FullName)) + ")";
            var methodInfo = FindMember(
                methodName,
                t => t.GetMethod(methodName, _BindingFlags, null, argTypes, null),
                memberSufix: memberSufix);
            return methodInfo.Invoke(_Target, args);
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
            if (_Cache == null) return null;
            var cacheOfMe = default(IDictionary<string, MemberInfo>);
            lock (_Cache)
            {
                if (_Cache.TryGetValue(_TypeOfTarget, out cacheOfMe) == false)
                {
                    cacheOfMe = new Dictionary<string, MemberInfo>();
                    _Cache.Add(_TypeOfTarget, cacheOfMe);
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
                var memberInfo = EnumType(_TypeOfTarget)
                    .Select(t => finder(t))
                    .FirstOrDefault(m => m != null);
                if (memberInfo == null && throwExceptionIfMemberNotFound)
                    throw new Exception("Member " + memberName + memberSufix + " not found.");
                return memberInfo;
            };

            var cacheOfMe = GetCacheOfMe();
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
                _Binder = accessor;
            }

            public PropertyInfo GetInfo(string propName, bool throwExceptionIfMemberNotFound = true)
            {
                return _Binder.FindMember(propName, t => t.GetProperty(propName, _Binder._BindingFlags), throwExceptionIfMemberNotFound);
            }

            public object this[string propName]
            {
                get
                {
                    var propInfo = GetInfo(propName);
                    return propInfo.GetValue(_Binder._Target, null);
                }
                set
                {
                    var propInfo = GetInfo(propName);
                    propInfo.SetValue(_Binder._Target, value, null);
                }
            }

            public bool Has(string propName)
            {
                return GetInfo(propName, false) != null;
            }
        }

        public class FieldBinder
        {
            protected LateBinder _Binder;

            internal FieldBinder(LateBinder accessor)
            {
                _Binder = accessor;
            }

            public FieldInfo GetInfo(string fieldName, bool throwExceptionIfMemberNotFound = true)
            {
                return _Binder.FindMember(fieldName, t => t.GetField(fieldName, _Binder._BindingFlags), throwExceptionIfMemberNotFound);
            }

            public object this[string fieldName]
            {
                get
                {
                    var fieldInfo = GetInfo(fieldName);
                    return fieldInfo.GetValue(_Binder._Target);
                }
                set
                {
                    var fieldInfo = GetInfo(fieldName);
                    fieldInfo.SetValue(_Binder._Target, value);
                }
            }

            public bool Has(string fieldName)
            {
                return GetInfo(fieldName, false) != null;
            }
        }
    }
}
