using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 414 // disable warning CS0414: The field '*' is assigned but its value is never used.

namespace DynamicBinderTest
{
    public class TestTargetClass
    {
        // Instance members
        // ==============

        private string PropA { get; set; }

        private DateTime _FieldB = DateTime.Parse("2014/02/13 14:27:56");

        public TestTargetClass()
        {
            PropA = "Fizz";
        }

        private string MethodC(string name)
        {
            return "Method-C by string: " + name;
        }

        private int MethodC(int age)
        {
            return age;
        }

        // Static members
        // ==============

        private static string PropD { get; set; }

        private static string _FieldE = "Static Foo";

        private static string MethodF(string name, int age)
        {
            return "Method-F(int): " + name + " / " + age.ToString();
        }

        private static string MethodF(string name, double age)
        {
            return "Method-F(double): " + name + " / " + age.ToString();
        }
    }
}
