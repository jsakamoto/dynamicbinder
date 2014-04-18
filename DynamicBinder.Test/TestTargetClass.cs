using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 414 // disable warning CS0414: The field '*' is assigned but its value is never used.

namespace DynamicBinderTest
{
    public class TestTargetClass
    {
        public class SubItemClass1
        {
            public string Name { get; set; }
            private int Value { get; set; }
            public SubItemClass1(string name, int value) { Name = name; Value = value; }
        }

        private class SubItemClass2
        {
            public string Name { get; set; }
            private int Value { get; set; }
            public SubItemClass2(string name, int value) { Name = name; Value = value; }
        }

        // Instance members
        // ==============

        private string PropA { get; set; }

        private DateTime _FieldB = DateTime.Parse("2014/02/13 14:27:56");

        private SubItemClass1 PropG { get; set; }

        private SubItemClass2 PropH { get; set; }
        
        /// <summary>Constractor</summary>
        public TestTargetClass()
        {
            PropA = "Fizz";
            PropG = new SubItemClass1("Sam", 33);
            PropH = new SubItemClass2("Alice", 29);
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

        private static SubItemClass2 GetSubItem()
        {
            return new SubItemClass2("John", 40);
        }
    }

    public class DerivedTestTargetClass : TestTargetClass
    {
    }
}
