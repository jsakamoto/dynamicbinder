using System.Reflection;
using Toolbelt;
using Toolbelt.DynamicBinderExtension;
using Xunit;

namespace DynamicBinderTest;

public class LateBinderTest
{
    [Fact]
    public void CreateInstanceT_for_public_constructor()
    {
        var obj = LateBinder.CreateInstance<TestTargetClass>();
        obj.Prop["PropA"].Is("Fizz");
        ((DateTime)obj.Field["_FieldB"]).ToString("yyyy/MM/dd HH:mm:ss").Is("2014/02/13 14:27:56");
    }

    [Fact]
    public void CreateInstanceT_for_private_constructor_1()
    {
        var obj = LateBinder.CreateInstance<TestTargetClass>("Lorem");
        obj.Prop["PropA"].Is("Lorem");
        ((DateTime)obj.Field["_FieldB"]).ToString("yyyy/MM/dd HH:mm:ss").Is("2014/02/13 14:27:56");
    }

    [Fact]
    public void CreateInstanceT_for_private_constructor_2()
    {
        var obj = LateBinder.CreateInstance<TestTargetClass>("Ipsum", DateTime.Parse("2022/03/27 15:49:10"));
        obj.Prop["PropA"].Is("Ipsum");
        ((DateTime)obj.Field["_FieldB"]).ToString("yyyy/MM/dd HH:mm:ss").Is("2022/03/27 15:49:10");
    }

    [Fact]
    public void CreateInstance_for_public_constructor()
    {
        var obj = LateBinder.CreateInstance(typeof(TestTargetClass));
        obj.Prop["PropA"].Is("Fizz");
        ((DateTime)obj.Field["_FieldB"]).ToString("yyyy/MM/dd HH:mm:ss").Is("2014/02/13 14:27:56");
    }

    [Fact]
    public void CreateInstance_for_private_constructor_1()
    {
        var obj = LateBinder.CreateInstance(typeof(TestTargetClass), "Lorem");
        obj.Prop["PropA"].Is("Lorem");
        ((DateTime)obj.Field["_FieldB"]).ToString("yyyy/MM/dd HH:mm:ss").Is("2014/02/13 14:27:56");
    }

    [Fact]
    public void CreateInstance_for_private_constructor_2()
    {
        var obj = LateBinder.CreateInstance(typeof(TestTargetClass), "Ipsum", DateTime.Parse("2022/03/27 15:49:10"));
        obj.Prop["PropA"].Is("Ipsum");
        ((DateTime)obj.Field["_FieldB"]).ToString("yyyy/MM/dd HH:mm:ss").Is("2022/03/27 15:49:10");
    }

    [Fact]
    public void CreateInstance_with_DynamicObject_by_Dynamic()
    {
        // Given
        var testTarget = LateBinder.CreateInstance<TestTargetClass>();
        var subItemA = testTarget.Call("CreateSubItem1").ToLateBind();

        // When
        var subItemB = LateBinder.CreateInstance<TestTargetClass.SubItemClass1>(subItemA);

        // Then
        subItemB.Prop["Name"].Is("Jude");
        subItemB.Prop["Value"].Is(47);
    }

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

    [Fact]
    public void CallPrivateInstanceMethod_with_ref_and_out_Argument_by_LateBinder()
    {
        object obj = new TestTargetClass();

        var args = new object[] { 3, 4, default(int) };
        obj.ToLateBind().Call("MethodG", args);

        args[1].Is(5);
        args[2].Is(12);
    }

    [Fact]
    public void CallPrivateStaticMethod_with_ref_and_out_Argument_by_LateBinder()
    {
        var binder = LateBinder.Create<TestTargetClass>();

        var args = new object[] { 3, 4, default(int) };
        binder.Call("MethodH", args);

        args[1].Is(10);
        args[2].Is(6);
    }

    [Fact]
    public void RetrieveObject_and_UseIt_by_LateBinder()
    {
        // Given
        var binder = LateBinder.Create<TestTargetClass>();
        var subItem = binder.Call("GetSubItem").ToLateBind();

        // When
        var name = binder.Call("GetNameOfSubItem", subItem) as string;

        // Then
        name.Is("John");
        ((int)subItem.Prop["Value"]).Is(40);
    }
}
