﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Toolbelt;
using Toolbelt.DynamicBinderExtension;
using Xunit;

namespace DynamicBinderTest
{
    public class LateBinderTest
    {
        [Fact]
        public void CallOverloadedPrivateInstanceMethod_by_LateBinder()
        {
            object obj = new TestTargetClass();

            obj.ToLateBind().Call("MethodC", "Adelie")
                .IsInstanceOf<string>()
                .Is("Method-C by string: Adelie");

            obj.ToLateBind().Call("MethodC", 27)
                .IsInstanceOf<int>()
                .Is(27);
        }

        [Fact]
        public void GetAndSetPrivateInstanceProperty_by_LateBinder()
        {
            object obj = new TestTargetClass();
            obj.ToLateBind().Prop["PropA"]
                .IsInstanceOf<string>()
                .Is("Fizz");

            obj.ToLateBind().Prop["PropA"] = "Buzz";

            obj.ToLateBind().Prop["PropA"]
                .Is("Buzz");
        }

        [Fact]
        public void GetAndSetPrivateInstanceField_by_LateBinder()
        {
            object obj = new TestTargetClass();
            obj.ToLateBind().Field["_FieldB"]
                .IsInstanceOf<DateTime>()
                .Is(DateTime.Parse("2014/02/13 14:27:56"));

            obj.ToLateBind().Field["_FieldB"] = DateTime.Parse("2015/11/09 02:06:03");

            obj.ToLateBind().Field["_FieldB"]
                .Is(DateTime.Parse("2015/11/09 02:06:03"));
        }

        [Fact]
        public void CallOverloadedPrivateStaticMethod_by_LateBinder()
        {
            var binder = LateBinder.Create(typeof(TestTargetClass));

            binder.Call("MethodF", "Gentoo", 18)
                .IsInstanceOf<string>()
                .Is("Method-F(int): Gentoo / 18");

            binder.Call("MethodF", "RockHoper", 31.4)
                .IsInstanceOf<string>()
                .Is("Method-F(double): RockHoper / 31.4");
        }

        [Fact]
        public void GetAndSetPrivateStaticProperty_by_LateBinder()
        {
            var binder = LateBinder.Create<TestTargetClass>();
            binder.Prop["PropD"]
                .IsNull();

            try
            {
                binder.Prop["PropD"] = "FizzBuzz";

                binder.Prop["PropD"]
                    .IsInstanceOf<string>()
                    .Is("FizzBuzz");
            }
            finally
            {
                binder.Prop["PropD"] = null;
            }
        }

        [Fact]
        public void GetAndSetPrivateStaticField_by_LateBinder()
        {
            object obj = new TestTargetClass();
            var binder = LateBinder.Create(obj.GetType());

            binder.Field["_FieldE"]
                .IsInstanceOf<string>()
                .Is("Static Foo");

            try
            {
                binder.Field["_FieldE"] = "Static Bar";

                binder.Field["_FieldE"]
                    .Is("Static Bar");
            }
            finally
            {
                binder.Field["_FieldE"] = "Static Foo";
            }
        }

        // -------------------------

        [Fact]
        public void CallOverloadedPrivateInstanceMethod_of_DerivedClass_by_LateBinder()
        {
            object obj = new DerivedTestTargetClass();

            obj.ToLateBind().Call("MethodC", "Adelie")
                .IsInstanceOf<string>()
                .Is("Method-C by string: Adelie");

            obj.ToLateBind().Call("MethodC", 27)
                .IsInstanceOf<int>()
                .Is(27);
        }

        [Fact]
        public void GetAndSetPrivateInstanceProperty_of_DerivedClass_by_LateBinder()
        {
            object obj = new DerivedTestTargetClass();
            obj.ToLateBind().Prop["PropA"]
                .IsInstanceOf<string>()
                .Is("Fizz");

            obj.ToLateBind().Prop["PropA"] = "Buzz";

            obj.ToLateBind().Prop["PropA"]
                .Is("Buzz");
        }

        [Fact]
        public void GetAndSetPrivateInstanceField_of_DerivedClass_by_LateBinder()
        {
            object obj = new DerivedTestTargetClass();
            obj.ToLateBind().Field["_FieldB"]
                .IsInstanceOf<DateTime>()
                .Is(DateTime.Parse("2014/02/13 14:27:56"));

            obj.ToLateBind().Field["_FieldB"] = DateTime.Parse("2015/11/09 02:06:03");

            obj.ToLateBind().Field["_FieldB"]
                .Is(DateTime.Parse("2015/11/09 02:06:03"));
        }

        [Fact]
        public void CallOverloadedPrivateStaticMethod_of_DerivedClass_by_LateBinder()
        {
            var binder = LateBinder.Create(typeof(DerivedTestTargetClass));

            binder.Call("MethodF", "Gentoo", 18)
                .IsInstanceOf<string>()
                .Is("Method-F(int): Gentoo / 18");

            binder.Call("MethodF", "RockHoper", 31.4)
                .IsInstanceOf<string>()
                .Is("Method-F(double): RockHoper / 31.4");
        }

        [Fact]
        public void GetAndSetPrivateStaticProperty_of_DerivedClass_by_LateBinder()
        {
            var binder = LateBinder.Create<DerivedTestTargetClass>();
            binder.Prop["PropD"]
                .IsNull();

            try
            {
                binder.Prop["PropD"] = "FizzBuzz";

                binder.Prop["PropD"]
                    .IsInstanceOf<string>()
                    .Is("FizzBuzz");
            }
            finally
            {
                binder.Prop["PropD"] = null;
            }
        }

        [Fact]
        public void GetAndSetPrivateStaticField_of_DerivedClass_by_LateBinder()
        {
            object obj = new DerivedTestTargetClass();
            var binder = LateBinder.Create(obj.GetType());

            binder.Field["_FieldE"]
                .IsInstanceOf<string>()
                .Is("Static Foo");

            try
            {
                binder.Field["_FieldE"] = "Static Bar";

                binder.Field["_FieldE"]
                    .Is("Static Bar");
            }
            finally
            {
                binder.Field["_FieldE"] = "Static Foo";
            }
        }


        [Fact]
        public void CallOverloadedPrivateInstanceMethod_by_LateBinder_with_Cache()
        {
            object obj = new TestTargetClass();
            var cache = new Dictionary<Type, IDictionary<string, MemberInfo>>();

            obj.ToLateBind(cache).Call("MethodC", "Adelie")
                .IsInstanceOf<string>()
                .Is("Method-C by string: Adelie");

            obj.ToLateBind(cache).Call("MethodC", 27)
                .IsInstanceOf<int>()
                .Is(27);

            obj.ToLateBind(cache).Call("MethodC", "Emperor")
                .IsInstanceOf<string>()
                .Is("Method-C by string: Emperor");
        }
    }
}
