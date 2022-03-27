using Toolbelt;
using Toolbelt.DynamicBinderExtension;
using Xunit;

namespace DynamicBinderTest;

public class DynamicBinderTest
{
    [Fact]
    public void CreateInstanceT_for_public_constructor()
    {
        var obj = DynamicBinder.CreateInstance<TestTargetClass>();
        ((string)obj.PropA).Is("Fizz");
        ((DateTime)obj._FieldB).ToString("yyyy/MM/dd HH:mm:ss").Is("2014/02/13 14:27:56");
    }

    [Fact]
    public void CreateInstanceT_for_private_constructor_1()
    {
        var obj = DynamicBinder.CreateInstance<TestTargetClass>("Lorem");
        ((string)obj.PropA).Is("Lorem");
        ((DateTime)obj._FieldB).ToString("yyyy/MM/dd HH:mm:ss").Is("2014/02/13 14:27:56");
    }

    [Fact]
    public void CreateInstanceT_for_private_constructor_2()
    {
        var obj = DynamicBinder.CreateInstance<TestTargetClass>("Ipsum", DateTime.Parse("2022/03/27 15:49:10"));
        ((string)obj.PropA).Is("Ipsum");
        ((DateTime)obj._FieldB).ToString("yyyy/MM/dd HH:mm:ss").Is("2022/03/27 15:49:10");
    }

    [Fact]
    public void CreateInstance_for_public_constructor()
    {
        var obj = DynamicBinder.CreateInstance(typeof(TestTargetClass));
        ((string)obj.PropA).Is("Fizz");
        ((DateTime)obj._FieldB).ToString("yyyy/MM/dd HH:mm:ss").Is("2014/02/13 14:27:56");
    }

    [Fact]
    public void CreateInstance_for_private_constructor_1()
    {
        var obj = DynamicBinder.CreateInstance(typeof(TestTargetClass), "Lorem");
        ((string)obj.PropA).Is("Lorem");
        ((DateTime)obj._FieldB).ToString("yyyy/MM/dd HH:mm:ss").Is("2014/02/13 14:27:56");
    }

    [Fact]
    public void CreateInstance_for_private_constructor_2()
    {
        var obj = DynamicBinder.CreateInstance(typeof(TestTargetClass), "Ipsum", DateTime.Parse("2022/03/27 15:49:10"));
        ((string)obj.PropA).Is("Ipsum");
        ((DateTime)obj._FieldB).ToString("yyyy/MM/dd HH:mm:ss").Is("2022/03/27 15:49:10");
    }

    [Fact]
    public void CallOverloadedPrivateInstanceMethod_by_Dynamic()
    {
        object obj = new TestTargetClass();

        var actual1 = (string)obj.ToDynamic().MethodC("King");
        actual1.Is("Method-C by string: King");

        var actual2 = (int)obj.ToDynamic().MethodC(29);
        actual2.Is(29);
    }

    [Fact]
    public void GetAndSetPrivateInstanceProperty_by_Dynamic()
    {
        object obj = new TestTargetClass();
        var actual1 = (string)obj.ToDynamic().PropA;
        actual1.Is("Fizz");

        obj.ToDynamic().PropA = "Dynamic Buzz";

        var actual2 = (string)obj.ToDynamic().PropA;
        actual2.Is("Dynamic Buzz");
    }

    [Fact]
    public void GetAndSetPrivateInstanceField_by_Dynamic()
    {
        object obj = new TestTargetClass();
        var actual1 = (DateTime)obj.ToDynamic()._FieldB;
        actual1.Is(DateTime.Parse("2014/02/13 14:27:56"));

        obj.ToDynamic()._FieldB = DateTime.Parse("2016/12/10 03:07:04");

        var actual2 = (DateTime)obj.ToDynamic()._FieldB;
        actual2.Is(DateTime.Parse("2016/12/10 03:07:04"));
    }

    [Fact]
    public void CallOverloadedPrivateStaticMethod_by_Dynamic()
    {
        var binder = DynamicBinder.Create<TestTargetClass>();

        var actual1 = (string)binder.MethodF("Emperor", 46);
        actual1.Is("Method-F(int): Emperor / 46");

        var actual2 = (string)binder.MethodF("Strap", 19.28);
        actual2.Is("Method-F(double): Strap / 19.28");
    }

    [Fact]
    public void GetAndSetPrivateStaticProperty_by_Dynamic()
    {
        var binder = DynamicBinder.Create(typeof(TestTargetClass));
        var actual1 = (string)binder.PropD;
        actual1.IsNull();

        try
        {
            binder.PropD = "Dynamic FizzBuzz";

            var actual2 = (string)binder.PropD;
            actual2.Is("Dynamic FizzBuzz");
        }
        finally
        {
            binder.PropD = null;
        }
    }

    [Fact]
    public void GetAndSetPrivateStaticField_by_Dynamic()
    {
        var obj = new TestTargetClass();
        var binder = DynamicBinder.Create(obj.GetType());
        var actual1 = (string)binder._FieldE;
        actual1.Is("Static Foo");

        try
        {
            binder._FieldE = "Static Dynamic Bar";

            var actual2 = (string)binder._FieldE;
            actual2.Is("Static Dynamic Bar");
        }
        finally
        {
            binder._FieldE = "Static Foo";
        }
    }

    [Fact]
    public void RetrieveClassObject_by_Dynamic()
    {
        var binder = DynamicBinder.Create<TestTargetClass>();

        var subItem = binder.GetSubItem();
        ((string)subItem.Name).Is("John");
        ((int)subItem.Value).Is(40);
    }

    [Fact]
    public void GetAndSetPropOfClassObject_by_Dynamic()
    {
        object obj = new TestTargetClass();
        string name1 = obj.ToDynamic().PropH.Name;
        int value1 = obj.ToDynamic().PropH.Value;
        name1.Is("Alice");
        value1.Is(29);

        obj.ToDynamic().PropH.Name = "Bob";
        obj.ToDynamic().PropH.Value = 37;
        string name2 = obj.ToDynamic().PropH.Name;
        int value2 = obj.ToDynamic().PropH.Value;
        name2.Is("Bob");
        value2.Is(37);

        string name3 = obj.ToDynamic().PropG.Name;
        int value3 = obj.ToDynamic().PropG.Value;
        name3.Is("Sam");
        value3.Is(33);

        obj.ToDynamic().PropG = new TestTargetClass.SubItemClass1("Baby", 0);
        string name4 = obj.ToDynamic().PropG.Name;
        int value4 = obj.ToDynamic().PropG.Value;
        name4.Is("Baby");
        value4.Is(0);
    }

    public enum Gender { Male, Female }

    [Fact]
    public void GetPropOfAnonymousType_by_Dynamic()
    {
        object obj = new
        {
            Person = new
            {
                Gender = Gender.Male,
                Birthday = DateTime.Parse("1970/01/15"),
                ProgramingLangs = new[] { "C#", "F#" }
            },
            Count = 1
        };

        Gender gender = obj.ToDynamic().Person.Gender;
        DateTime birthday = obj.ToDynamic().Person.Birthday;
        int birthdayYear = obj.ToDynamic().Person.Birthday.Year;
        string birthdayMonth = obj.ToDynamic().Person.Birthday.ToLocalTime().Month.ToString();
        string[] programingLangs = obj.ToDynamic().Person.ProgramingLangs;
        int count = obj.ToDynamic().Count;

        gender.Is(Gender.Male);
        birthday.Is(DateTime.Parse("1970/01/15"));
        birthdayYear.Is(1970);
        birthdayMonth.Is("1");
        programingLangs.Is("C#", "F#");
        count.Is(1);
    }

    [Fact]
    public void RetrieveReturnValueOfMethod()
    {
        object obj = new TestTargetClass();
        var retval = obj.ToDynamic().CreateSubItem1() as DynamicBinder;
        retval.Object.ToString().Is("Good Job!");
    }

    // -------------------------

    [Fact]
    public void CallOverloadedPrivateInstanceMethod_of_DerivedClass_by_Dynamic()
    {
        object obj = new DerivedTestTargetClass();

        var actual1 = (string)obj.ToDynamic().MethodC("King");
        actual1.Is("Method-C by string: King");

        var actual2 = (int)obj.ToDynamic().MethodC(29);
        actual2.Is(29);
    }

    [Fact]
    public void GetAndSetPrivateInstanceProperty_of_DerivedClass_by_Dynamic()
    {
        object obj = new DerivedTestTargetClass();
        var actual1 = (string)obj.ToDynamic().PropA;
        actual1.Is("Fizz");

        obj.ToDynamic().PropA = "Dynamic Buzz";

        var actual2 = (string)obj.ToDynamic().PropA;
        actual2.Is("Dynamic Buzz");
    }

    [Fact]
    public void GetAndSetPrivateInstanceField_of_DerivedClass_by_Dynamic()
    {
        object obj = new DerivedTestTargetClass();
        var actual1 = (DateTime)obj.ToDynamic()._FieldB;
        actual1.Is(DateTime.Parse("2014/02/13 14:27:56"));

        obj.ToDynamic()._FieldB = DateTime.Parse("2016/12/10 03:07:04");

        var actual2 = (DateTime)obj.ToDynamic()._FieldB;
        actual2.Is(DateTime.Parse("2016/12/10 03:07:04"));
    }

    [Fact]
    public void CallOverloadedPrivateStaticMethod_of_DerivedClass_by_Dynamic()
    {
        var binder = DynamicBinder.Create<DerivedTestTargetClass>();

        var actual1 = (string)binder.MethodF("Emperor", 46);
        actual1.Is("Method-F(int): Emperor / 46");

        var actual2 = (string)binder.MethodF("Strap", 19.28);
        actual2.Is("Method-F(double): Strap / 19.28");
    }

    [Fact]
    public void GetAndSetPrivateStaticProperty_of_DerivedClass_by_Dynamic()
    {
        var binder = DynamicBinder.Create(typeof(DerivedTestTargetClass));
        var actual1 = (string)binder.PropD;
        actual1.IsNull();

        try
        {
            binder.PropD = "Dynamic FizzBuzz";

            var actual2 = (string)binder.PropD;
            actual2.Is("Dynamic FizzBuzz");
        }
        finally
        {
            binder.PropD = null;
        }
    }

    [Fact]
    public void GetAndSetPrivateStaticField_of_DerivedClass_by_Dynamic()
    {
        var obj = new DerivedTestTargetClass();
        var binder = DynamicBinder.Create(obj.GetType());
        var actual1 = (string)binder._FieldE;
        actual1.Is("Static Foo");

        try
        {
            binder._FieldE = "Static Dynamic Bar";

            var actual2 = (string)binder._FieldE;
            actual2.Is("Static Dynamic Bar");
        }
        finally
        {
            binder._FieldE = "Static Foo";
        }
    }
}
