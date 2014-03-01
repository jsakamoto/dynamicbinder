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

        public object Call(string methodName, params object[] args)
        {
            var types = args.Select(_ => _.GetType()).ToArray();
            var methodInfo = FindMember(t => t.GetMethod(methodName, _BindingFlags, null, types, null));
            return methodInfo.Invoke(_Target, args);
        }

        protected static IEnumerable<Type> EnumType(Type type)
        {
            for (; type != null; type = type.BaseType)
            {
                yield return type;
            }
        }

        internal T FindMember<T>(Func<Type, T> finder, bool throwExceptionIfMemberNotFound = true)
        {
            var member = EnumType(_TypeOfTarget)
                .Select(t => finder(t))
                .FirstOrDefault(m => m != null);
            if (member == null && throwExceptionIfMemberNotFound)
                throw new Exception("Member " + member + " not found.");
            return member;
        }

        public class PropertyBinder
        {
            protected LateBinder _Binder;

            internal PropertyBinder(LateBinder accessor)
            {
                _Binder = accessor;
            }

            public object this[string propName]
            {
                get
                {
                    var propInfo = _Binder.FindMember(t => t.GetProperty(propName, _Binder._BindingFlags));
                    return propInfo.GetValue(_Binder._Target, null);
                }
                set
                {
                    var propInfo = _Binder.FindMember(t => t.GetProperty(propName, _Binder._BindingFlags));
                    propInfo.SetValue(_Binder._Target, value, null);
                }
            }

            public bool Has(string propName)
            {
                return _Binder.FindMember(t => t.GetProperty(propName, _Binder._BindingFlags), false) != null;
            }
        }

        public class FieldBinder
        {
            protected LateBinder _Binder;

            internal FieldBinder(LateBinder accessor)
            {
                _Binder = accessor;
            }

            public object this[string fieldName]
            {
                get
                {
                    var fieldInfo = _Binder.FindMember(t => t.GetField(fieldName, _Binder._BindingFlags));
                    return fieldInfo.GetValue(_Binder._Target);
                }
                set
                {
                    var fieldInfo = _Binder.FindMember(t => t.GetField(fieldName, _Binder._BindingFlags));
                    fieldInfo.SetValue(_Binder._Target, value);
                }
            }

            public bool Has(string fieldName)
            {
                return _Binder.FindMember(t => t.GetField(fieldName, _Binder._BindingFlags), false) != null;
            }
        }
    }
}
