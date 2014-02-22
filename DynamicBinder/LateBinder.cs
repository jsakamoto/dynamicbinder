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
            var methodInfo = _TypeOfTarget.GetMethod(methodName, _BindingFlags, null, types, null);
            return methodInfo.Invoke(_Target, args);
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
                    var propInfo = _Binder._TypeOfTarget.GetProperty(propName, _Binder._BindingFlags);
                    return propInfo.GetValue(_Binder._Target, null);
                }
                set
                {
                    var propInfo = _Binder._TypeOfTarget.GetProperty(propName, _Binder._BindingFlags);
                    propInfo.SetValue(_Binder._Target, value, null);
                }
            }

            public bool Has(string propName)
            {
                return _Binder._TypeOfTarget.GetProperty(propName, _Binder._BindingFlags) != null;
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
                    var propInfo = _Binder._TypeOfTarget.GetField(fieldName, _Binder._BindingFlags);
                    return propInfo.GetValue(_Binder._Target);
                }
                set
                {
                    var propInfo = _Binder._TypeOfTarget.GetField(fieldName, _Binder._BindingFlags);
                    propInfo.SetValue(_Binder._Target, value);
                }
            }

            public bool Has(string fieldName)
            {
                return _Binder._TypeOfTarget.GetField(fieldName, _Binder._BindingFlags) != null;
            }
        }
    }
}
