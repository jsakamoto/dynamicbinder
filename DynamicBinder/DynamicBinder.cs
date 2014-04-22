using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Toolbelt
{
    public class DynamicBinder : DynamicObject
    {
        protected LateBinder _Binder;

        internal DynamicBinder(LateBinder accessor)
        {
            _Binder = accessor;
        }

        public DynamicBinder(object target)
        {
            _Binder = new LateBinder(target);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (base.TryInvokeMember(binder, args, out result)) return true;
            result = Wrap(_Binder.Call(binder.Name, args));
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (base.TryGetMember(binder, out result)) return true;
            if (_Binder.Prop.Has(binder.Name))
            {
                result = Wrap(_Binder.Prop[binder.Name]);
                return true;
            }
            else if(_Binder.Field.Has(binder.Name))
            {
                result = Wrap(_Binder.Field[binder.Name]);
                return true;
            }
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (base.TrySetMember(binder, value)) return true;
            if (_Binder.Prop.Has(binder.Name))
            {
                _Binder.Prop[binder.Name] = value;
                return true;
            }
            else if (_Binder.Field.Has(binder.Name))
            {
                _Binder.Field[binder.Name] = value;
                return true;
            }
            return false;
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
