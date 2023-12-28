#pragma warning disable 414 // disable warning CS0414: The field '*' is assigned but its value is never used.

namespace DynamicBinderTest;

public class TestTargetClass
{
    public class SubItemClass1
    {
        public string Name { get; set; }
        private int Value { get; set; }
        public SubItemClass1(string name, int value) { this.Name = name; this.Value = value; }
        public override string ToString() { return "Good Job!"; }
    }

    private class SubItemClass2
    {
        public string Name { get; set; }
        private int Value { get; set; }
        public SubItemClass2(string name, int value) { this.Name = name; this.Value = value; }
    }

    // Instance members
    // ==============

    private string PropA { get; set; }

    private DateTime _FieldB = DateTime.Parse("2014/02/13 14:27:56");

    private SubItemClass1 PropG { get; set; }

    private SubItemClass2 PropH { get; set; }

    /// <summary>
    /// Public Constractor
    /// </summary>
    public TestTargetClass()
    {
        this.PropA = "Fizz";
        this.PropG = new SubItemClass1("Sam", 33);
        this.PropH = new SubItemClass2("Alice", 29);
    }

    /// <summary>
    /// Private Constractor 1
    /// </summary>
    private TestTargetClass(string propA) : this()
    {
        this.PropA = propA;
    }

    /// <summary>
    /// Private Constractor 2
    /// </summary>
    private TestTargetClass(string propA, DateTime fieldB) : this(propA)
    {
        this._FieldB = fieldB;
    }

    private string MethodC(string name)
    {
        return "Method-C by string: " + name;
    }

    private int MethodC(int age)
    {
        return age;
    }

    private void MethodG(int x, ref int y, out int z)
    {
        z = x * y;
        y = y + 1;
    }

    public SubItemClass1 CreateSubItem1()
    {
        return new SubItemClass1("Jude", 47);
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

    private static void MethodH(int x, ref int y, out int z)
    {
        z = x * 2;
        y = y + z;
    }

    private static SubItemClass2 GetSubItem()
    {
        return new SubItemClass2("John", 40);
    }
}

public class DerivedTestTargetClass : TestTargetClass
{
}
