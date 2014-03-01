using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Toolbelt;
using Toolbelt.DynamicBinderExtension;

namespace DynamicBinderTest
{
    [TestClass]
    public class DynamicBinderTest
    {
        [TestMethod]
        public void CallOverloadedPrivateInstanceMethod_by_Dynamic()
        {
            object obj = new TestTargetClass();

            var actual1 = (string)obj.ToDynamic().MethodC("King");
            actual1.Is("Method-C by string: King");

            var actual2 = (int)obj.ToDynamic().MethodC(29);
            actual2.Is(29);
        }

        [TestMethod]
        public void GetAndSetPrivateInstanceProperty_by_Dynamic()
        {
            object obj = new TestTargetClass();
            var actual1 = (string)obj.ToDynamic().PropA;
            actual1.Is("Fizz");

            obj.ToDynamic().PropA = "Dynamic Buzz";

            var actual2 = (string)obj.ToDynamic().PropA;
            actual2.Is("Dynamic Buzz");
        }

        [TestMethod]
        public void GetAndSetPrivateInstanceField_by_Dynamic()
        {
            object obj = new TestTargetClass();
            var actual1 = (DateTime)obj.ToDynamic()._FieldB;
            actual1.Is(DateTime.Parse("2014/02/13 14:27:56"));

            obj.ToDynamic()._FieldB = DateTime.Parse("2016/12/10 03:07:04");

            var actual2 = (DateTime)obj.ToDynamic()._FieldB;
            actual2.Is(DateTime.Parse("2016/12/10 03:07:04"));
        }

        [TestMethod]
        public void CallOverloadedPrivateStaticMethod_by_Dynamic()
        {
            var accessor = DynamicBinder.Create<TestTargetClass>();

            var actual1 = (string)accessor.MethodF("Emperor", 46);
            actual1.Is("Method-F(int): Emperor / 46");

            var actual2 = (string)accessor.MethodF("Strap", 19.28);
            actual2.Is("Method-F(double): Strap / 19.28");
        }

        [TestMethod]
        public void GetAndSetPrivateStaticProperty_by_Dynamic()
        {
            var accessor = DynamicBinder.Create(typeof(TestTargetClass));
            var actual1 = (string)accessor.PropD;
            actual1.IsNull();

            try
            {
                accessor.PropD = "Dynamic FizzBuzz";

                var actual2 = (string)accessor.PropD;
                actual2.Is("Dynamic FizzBuzz");
            }
            finally
            {
                accessor.PropD = null;
            }
        }

        [TestMethod]
        public void GetAndSetPrivateStaticField_by_Dynamic()
        {
            var obj = new TestTargetClass();
            var accessor = DynamicBinder.Create(obj.GetType());
            var actual1 = (string)accessor._FieldE;
            actual1.Is("Static Foo");

            try
            {
                accessor._FieldE = "Static Dynamic Bar";

                var actual2 = (string)accessor._FieldE;
                actual2.Is("Static Dynamic Bar");
            }
            finally
            {
                accessor._FieldE = "Static Foo";
            }
        }

        // -------------------------

        [TestMethod]
        public void CallOverloadedPrivateInstanceMethod_of_DerivedClass_by_Dynamic()
        {
            object obj = new DerivedTestTargetClass();

            var actual1 = (string)obj.ToDynamic().MethodC("King");
            actual1.Is("Method-C by string: King");

            var actual2 = (int)obj.ToDynamic().MethodC(29);
            actual2.Is(29);
        }

        [TestMethod]
        public void GetAndSetPrivateInstanceProperty_of_DerivedClass_by_Dynamic()
        {
            object obj = new DerivedTestTargetClass();
            var actual1 = (string)obj.ToDynamic().PropA;
            actual1.Is("Fizz");

            obj.ToDynamic().PropA = "Dynamic Buzz";

            var actual2 = (string)obj.ToDynamic().PropA;
            actual2.Is("Dynamic Buzz");
        }

        [TestMethod]
        public void GetAndSetPrivateInstanceField_of_DerivedClass_by_Dynamic()
        {
            object obj = new DerivedTestTargetClass();
            var actual1 = (DateTime)obj.ToDynamic()._FieldB;
            actual1.Is(DateTime.Parse("2014/02/13 14:27:56"));

            obj.ToDynamic()._FieldB = DateTime.Parse("2016/12/10 03:07:04");

            var actual2 = (DateTime)obj.ToDynamic()._FieldB;
            actual2.Is(DateTime.Parse("2016/12/10 03:07:04"));
        }

        [TestMethod]
        public void CallOverloadedPrivateStaticMethod_of_DerivedClass_by_Dynamic()
        {
            var accessor = DynamicBinder.Create<DerivedTestTargetClass>();

            var actual1 = (string)accessor.MethodF("Emperor", 46);
            actual1.Is("Method-F(int): Emperor / 46");

            var actual2 = (string)accessor.MethodF("Strap", 19.28);
            actual2.Is("Method-F(double): Strap / 19.28");
        }

        [TestMethod]
        public void GetAndSetPrivateStaticProperty_of_DerivedClass_by_Dynamic()
        {
            var accessor = DynamicBinder.Create(typeof(DerivedTestTargetClass));
            var actual1 = (string)accessor.PropD;
            actual1.IsNull();

            try
            {
                accessor.PropD = "Dynamic FizzBuzz";

                var actual2 = (string)accessor.PropD;
                actual2.Is("Dynamic FizzBuzz");
            }
            finally
            {
                accessor.PropD = null;
            }
        }

        [TestMethod]
        public void GetAndSetPrivateStaticField_of_DerivedClass_by_Dynamic()
        {
            var obj = new DerivedTestTargetClass();
            var accessor = DynamicBinder.Create(obj.GetType());
            var actual1 = (string)accessor._FieldE;
            actual1.Is("Static Foo");

            try
            {
                accessor._FieldE = "Static Dynamic Bar";

                var actual2 = (string)accessor._FieldE;
                actual2.Is("Static Dynamic Bar");
            }
            finally
            {
                accessor._FieldE = "Static Foo";
            }
        }
    }
}
