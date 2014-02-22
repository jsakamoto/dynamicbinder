using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toolbelt;
using Toolbelt.DynamicBinderExtension;

namespace DynamicBinderTest
{
    [TestClass]
    public class LateBinderTest
    {
        [TestMethod]
        public void CallOverloadedPrivateInstanceMethod_by_Accessor()
        {
            object obj = new TestTargetClass();
            
            obj.ToLateBind().Call("MethodC", "Adelie")
                .IsInstanceOf<string>()
                .Is("Method-C by string: Adelie");

            obj.ToLateBind().Call("MethodC", 27)
                .IsInstanceOf<int>()
                .Is(27);
        }

        [TestMethod]
        public void GetAndSetPrivateInstanceProperty_by_Accessor()
        {
            object obj = new TestTargetClass();
            obj.ToLateBind().Prop["PropA"]
                .IsInstanceOf<string>()
                .Is("Fizz");

            obj.ToLateBind().Prop["PropA"] = "Buzz";

            obj.ToLateBind().Prop["PropA"]
                .Is("Buzz");
        }

        [TestMethod]
        public void GetAndSetPrivateInstanceField_by_Accessor()
        {
            object obj = new TestTargetClass();
            obj.ToLateBind().Field["_FieldB"]
                .IsInstanceOf<DateTime>()
                .Is(DateTime.Parse("2014/02/13 14:27:56"));

            obj.ToLateBind().Field["_FieldB"] = DateTime.Parse("2015/11/09 02:06:03");

            obj.ToLateBind().Field["_FieldB"]
                .Is(DateTime.Parse("2015/11/09 02:06:03"));
        }

        [TestMethod]
        public void CallOverloadedPrivateStaticMethod_by_Accessor()
        {
            var accessor = LateBinder.Create(typeof(TestTargetClass));
            
            accessor.Call("MethodF", "Gentoo", 18)
                .IsInstanceOf<string>()
                .Is("Method-F(int): Gentoo / 18");

            accessor.Call("MethodF", "RockHoper", 31.4)
                .IsInstanceOf<string>()
                .Is("Method-F(double): RockHoper / 31.4");
        }

        [TestMethod]
        public void GetAndSetPrivateStaticProperty_by_Accessor()
        {
            var accessor = LateBinder.Create<TestTargetClass>();
            accessor.Prop["PropD"]
                .IsNull();

            try
            {
                accessor.Prop["PropD"] = "FizzBuzz";

                accessor.Prop["PropD"]
                    .IsInstanceOf<string>()
                    .Is("FizzBuzz");
            }
            finally
            {
                accessor.Prop["PropD"] = null;
            }
        }

        [TestMethod]
        public void GetAndSetPrivateStaticField_by_Accessor()
        {
            object obj = new TestTargetClass();
            var accessor = LateBinder.Create(obj.GetType());

            accessor.Field["_FieldE"]
                .IsInstanceOf<string>()
                .Is("Static Foo");

            try
            {
                accessor.Field["_FieldE"] = "Static Bar";

                accessor.Field["_FieldE"]
                    .Is("Static Bar");
            }
            finally
            {
                accessor.Field["_FieldE"] = "Static Foo";
            }
        }
    }
}
