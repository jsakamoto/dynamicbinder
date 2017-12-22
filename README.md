DynamicBinder and LateBinder [![NuGet Package](https://img.shields.io/nuget/v/DynamicBinder.svg)](https://www.nuget.org/packages/DynamicBinder/)
============================

What's this?
------------
This is the class library for .NET.

This library allows you to dynamic access to object methods, properties, and fields 
even if they are private members by Reflection technology.

You can access the both of object instance members and class static members by name that specified string argument at runtime not compile time, or C# 4.0 "dynamic" syntax.

How to install?
---------------
You can install this library via [NuGet](https://www.nuget.org/packages/DynamicBinder/).

    PM> Install-Package DynamicBinder

How to use?
------------

### C# "dynamic" syntax

#### Instance member access

After import(open) namespace "Toolbelt.DynamicBinderExtension",
you can use ```ToDynamic()``` extension method that returned
C#4.0 "dynamic" type at any object.

```C#
using Toolbelt.DynamicBinderExtension;
...
// call instance method.
var retval = (int)obj.ToDynamic().MethodName(arg1, arg2);

// get & set instance property.
var value = (int)obj.ToDynamic().PropName;
obj.ToDynamic().PropName = newValue;

// get & set instance field.
var value = (int)obj.ToDynamic().FieldName;
obj.ToDynamic().FieldName = newValue;
```

#### Static member access

After import(open) namespace "Toolbelt",
you can use ```DynamicBinder.Create<T>()``` and 
 ```DynamicBinder.Create(Type t)``` static method that returned
C#4.0 "dynamic" type.

```C#
using Toolbelt;
...
var binder = DynamicBinder.Create(typeof(Foo));
// call static method.
var retval = (int)binder.MethodName(arg1, arg2);

// get & set static property.
var value = (int)binder.PropName;
binder.PropName = newValue;

// get & set static field.
var value = (int)binder.FieldName;
binder.FieldName = newValue;
```

### Notice: retrieve class type return value from method calling

the follow test code is fail.

```C#
object bar = foo.ToDynamic().GetBarObject();
Assert.AreEqual("BarClass", bar.GetType().Name); // report actual is "DynamicBinder"!
```

Because C# dynamic conversion type to object is return DynamicBinder object it self.

You should rewrite avobe test code as follow.

```C#
// etract C# dynamic object to DynamicBinder object, static type world.
var retval = foo.ToDynamic().GetBarObject() as DynamicBinder;
// DynamicBinder class exposed "Object" property to access the binding target object.
Assert.AreEqual("BarClass", retval.Object.GetType().Name); // Green. Pass!
```

Of course, if you have the right type information, thease test codes can write follow:

```C#
var bar = (BarClass)foo.ToDynamic().GetBarObject();
Assert.AreEqual("BarClass", bar.GetType().Name); // Green. Pass!
```


### Late bind syntax

#### Instance member access

After import(open) namespace "Toolbelt.DynamicBinderExtension",
you can use ```ToLateBind()``` extension method that returned 
"LateBinder" object at any object.

"LateBinder" has follow members.

---

-  ```Call(name, paams[] args)``` method
- ```Prop[name]``` property
- ```Field[name]``` property

---
```C#
using Toolbelt.DynamicBinderExtension;
...
// call method.
var retval = (int)obj.ToLateBind().Call("MethodName", arg1, arg2);

// get & set property.
var value = (int)obj.ToLateBind().Prop["PropName"];
obj.ToLateBind().Prop["PropName"] = newValue;

// get & set field.
var value = (int)obj.ToLateBind().Field["FieldName"];
obj.ToLateBind().Field["FieldName"] = newValue;
```

#### Static member access

After import(open) namespace "Toolbelt",
you can use ```LateBinder.Create<T>()``` and 
```LateBinder.Create(Type t)``` static method that returned
"LateBinder" object.

```C#
using Toolbelt;
...
var binder = LateBinder.Create<Foo>();
// call static method.
var retval = (int)binder.Call("MethodName", arg1, arg2);

// get & set static property.
var value = (int)binder.Prop["PropName"];
binder.Prop["PropName"] = newValue;

// get & set static field.
var value = (int)binder.Field["FieldName"];
binder.Field["FieldName"] = newValue;
```

### No use extension methods

If you feel these extension method is dirty, you can chose no using these extension method.

Instead, you can use LateBinder class and DynamicBinder class like follow code.

```C#
// do not open namespace "Toolbelt.DynamicBinderExtension".
using Toolbelt;
...
dynamic dynamicBinder = new DynamicBinder(obj);
var retval = (int)dynamicBinder.MethodName(arg1, arg2);

var lateBinder = new LateBinder(obj);
var retval = (int)lateBinder.Call("MethodName", arg1, arg2);
```

Note
------

### "reinventing the wheel"
There are many many packages more than 50 about reflection & private member accessing library.

https://www.nuget.org/packages?q=Tags%3A%22reflection%22

But I couldn't find any library which has my favorit syntax and features :).

So I deside to "reinvent the wheel".


### Performance issue

This library "DynamicBinder" and "LateBinder" may be very slowly because the implementation of
this library is calling reflection API directory without chache, compile to delegate, complile to expression,
and so on.

There is plenty room for improvement to more faster, more high performance.

If you prefer, you can fork this source codes and improve it.
