using System;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using Toolbelt.DynamicBinderExtension;

namespace Toolbelt
{
    public class DynamicBinder : DynamicObject
    {
        protected LateBinder _Binder;

        internal DynamicBinder(LateBinder accessor)
        {
            this._Binder = accessor;
        }

        public static dynamic CreateInstance<T>(params object[] args)
        {
            return CreateInstance(typeof(T), args);
        }

        public static dynamic CreateInstance(Type type, params object[] args)
        {
            var obj = Activator.CreateInstance(type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, args, default(CultureInfo));
            return obj.ToDynamic();
        }

        public DynamicBinder(object target)
        {
            this._Binder = new LateBinder(target);
        }

        /// <summary>get the object that dynamic binding taret.</summary>
        public object Object { get { return this._Binder.Object; } }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (base.TryInvokeMember(binder, args, out result)) return true;
            result = Wrap(this._Binder.Call(binder.Name, args));
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (base.TryGetMember(binder, out result)) return true;
            if (this._Binder.Prop.Has(binder.Name))
            {
                result = Wrap(this._Binder.Prop[binder.Name]);
                return true;
            }
            else if (this._Binder.Field.Has(binder.Name))
            {
                result = Wrap(this._Binder.Field[binder.Name]);
                return true;
            }
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (base.TrySetMember(binder, value)) return true;
            if (this._Binder.Prop.Has(binder.Name))
            {
                this._Binder.Prop[binder.Name] = value;
                return true;
            }
            else if (this._Binder.Field.Has(binder.Name))
            {
                this._Binder.Field[binder.Name] = value;
                return true;
            }
            return false;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (base.TryConvert(binder, out result)) return true;
            result = this._Binder.Object;
            return true;
        }

        private static object Wrap(object obj)
        {
            return obj == null ? null :
                Type.GetTypeCode(obj.GetType()) == TypeCode.Object ?
                new DynamicBinder(obj) : obj;
        }

        public static dynamic Create<T>()
        {
            return new DynamicBinder(LateBinder.Create<T>());
        }

        public static dynamic Create(Type type)
        {
            return new DynamicBinder(LateBinder.Create(type));
        }
    }
}
