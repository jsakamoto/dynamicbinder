# DynamicBinder and LateBinder [![NuGet Package](https://img.shields.io/nuget/v/DynamicBinder.svg)](https://www.nuget.org/packages/DynamicBinder/) [![unit tests](https://github.com/jsakamoto/dynamicbinder/actions/workflows/unit-tests.yml/badge.svg)](https://github.com/jsakamoto/dynamicbinder/actions/workflows/unit-tests.yml)

## What's this?

This is the class library for .NET.

This library allows you dynamic access to object methods, properties, and fields using the reflection technology of .NET, regardless of whether they are private members.

You can access both objects' instance members and classes' static members by name that specified string argument at runtime, not compile-time, or C# 4.0 "dynamic" syntax.

## How to install?

You can install this library via [NuGet](https://www.nuget.org/packages/DynamicBinder/).

```shell
> dotnet add package DynamicBinder
```

## Usage - C# "dynamic" syntax

### Create instance

After importing (opening) namespace `Toolbelt`, you can use `DynamicBinder.CreateInstance<T>(...)` and `DynamicBinder.CreateInstance(Type t, ...)` static method to instantiate any objects with any constructors, regardless of constructor's access level (public, internal, protected, private).

Those methods return an instantiated object wrapped with the `DynamicBinder` object as a `dynamic` type.

```csharp
using Toolbelt;
...
// 👇 The type of the "dynamicObj" is the "dynamic" type.
//    In this case, the "dynamicObj" is instantiated by the constructor, which has two arguments.
//    It can be instantiated even if the constructor is private.
var dynamicObj = DynamicBinder.CreateInstance<MyClass>(arg1, arg2);

// And it can be invoked its instance methods, regardless of its access level.
var retval = (int)dynamicObj.PrivateMethodName(arg3, arg4);
...
```

### Access to instance members

After importing (opening) namespace `Toolbelt.DynamicBinderExtension`, you can use `ToDynamic()` extension method that returned C #4.0 `dynamic` type at any object.

```csharp
using Toolbelt.DynamicBinderExtension;
...
var obj = new MyClass();
...
// Call an instance method.
// (You can pass ref & out arguments.)
var retval = (int)obj.ToDynamic().MethodName(arg1, ref arg2, out int arg3);

// Get or set an instance property.
var value = (int)obj.ToDynamic().PropName;
obj.ToDynamic().PropName = newValue;

// Get or set an instance field.
var value = (int)obj.ToDynamic().FieldName;
obj.ToDynamic().FieldName = newValue;
```

### Access to static members

After importing (opening) namespace `Toolbelt`, you can use `DynamicBinder.Create<T>()` and `DynamicBinder.Create(Type t)` static method that returned C #4.0 `dynamic` type.

```csharp
using Toolbelt;
...
var binder = DynamicBinder.Create(typeof(Foo));

// Call a static method.
// (You can pass ref & out arguments.)
var retval = (int)binder.MethodName(arg1, ref arg2, out int arg3);

// Get or set a static property.
var value = (int)binder.PropName;
binder.PropName = newValue;

// Get or set a static field.
var value = (int)binder.FieldName;
binder.FieldName = newValue;
```

### NOTICE: Retrieving a type information of the returned value from the method calling

Retrieving a type information of the return value from the method calling

The following test code will fail.

```csharp
object bar = foo.ToDynamic().GetBarObject();

// 👇 It will report "actual is `DynamicBinder`" !
Assert.AreEqual("BarClass", bar.GetType().Name); 
```

You should rewrite the above test code as follows.

```csharp
// Extract a `DynamicBinder` object from the C# dynamic object by casting with `as`.
var retval = foo.ToDynamic().GetBarObject() as DynamicBinder;

// The `DynamicBinder` class exposes the `Object` property to access the bound target object.
Assert.AreEqual("BarClass", retval.Object.GetType().Name); // Green. Pass!
```

Of course, if you have the right type information, those test codes can be rewritten as the following:

```csharp
var bar = (BarClass)foo.ToDynamic().GetBarObject();
Assert.AreEqual("BarClass", bar.GetType().Name); // Green. Pass!
```


## Usage - Late bind syntax

### Create instance

After importing (opening) namespace `Toolbelt`, you can use `LateBinder.CreateInstance<T>(...)` and `LateBinder.CreateInstance(Type t, ...)` static method to instantiate any objects with any constructors, regardless of constructor's access level (public, internal, protected, private).

Those methods return an instantiated object wrapped with the `LateBinder` type.

```csharp
using Toolbelt;
...
// 👇 The type of the "dynamicObj" is the "LateBinder" type.
//    In this case, the "dynamicObj" is instantiated by the constructor, which has two arguments.
//    It can be instantiated even if the constructor is private.
var dynamicObj = LateBinder.CreateInstance<MyClass>(arg1, arg2);

// And it can be invoked its instance methods, regardless of its access level.
var retval = (int)dynamicObj.Call("PrivateMethodName", arg3, arg4);
...
```

### Access to instance members

After importing (opening) namespace `Toolbelt.DynamicBinderExtension`, you can use `ToLateBind()` extension method that returned `LateBinder` object at any object.

`LateBinder` has follow members.

- `Call(name, params[] args)` method
- `Prop[name]` property
- `Field[name]` property

```csharp
using Toolbelt.DynamicBinderExtension;
...
// Call an instance method.
var retval = (int)obj.ToLateBind().Call("MethodName", arg1, arg2);

// Get or set an instance property.
var value = (int)obj.ToLateBind().Prop["PropName"];
obj.ToLateBind().Prop["PropName"] = newValue;

// Get or set an instance field.
var value = (int)obj.ToLateBind().Field["FieldName"];
obj.ToLateBind().Field["FieldName"] = newValue;
```

### Access to static members

After importing (opening) namespace `Toolbelt`, you can use `LateBinder.Create<T>()` and `LateBinder.Create(Type t)` static method that returned `LateBinder` object.

```csharp
using Toolbelt;
...
var binder = LateBinder.Create<Foo>();
// Call a static method.
var retval = (int)binder.Call("MethodName", arg1, arg2);

// Get or set a static property.
var value = (int)binder.Prop["PropName"];
binder.Prop["PropName"] = newValue;

// Get or set a static field.
var value = (int)binder.Field["FieldName"];
binder.Field["FieldName"] = newValue;
```

### Call methods with `ref` & `out` arguments

To call methods with `ref` & `out` arguments, you can't pass those arguments to the `Call` method of the late-binder directly, which is different from using the "dynamic" syntax. Instead, follow the instructions below.

```csharp
// For example, if the definition of the "MethodName" static method is:

// static void MethodName(int x, ref int y, out int z){
//    z = x * y;
//    y = y + 1;
// }

// 1. Pack all of the arguments into an object array.
var args = new object[] { 3, 4, default(int) };

// 2. Pass it to the 2nd argument of the "Call" method.
binder.Call("MethodName", args);

// 3. Then, the "args" array will be updated.
// args[1] -> 5
// args[2] -> 12
```

## Note

### No using extension methods scenario

If you feel these extension methods are dirty, you can choose no using these extension methods.

Instead, you can use `LateBinder` and `DynamicBinder` class like the following code.

```csharp
// Do not open namespace "Toolbelt.DynamicBinderExtension".
using Toolbelt;
...
// Instead, instantiate DynamicBinder or LateBinder objects with the "new" keyword.
dynamic dynamicBinder = new DynamicBinder(obj);
var retval = (int)dynamicBinder.MethodName(arg1, arg2);

var lateBinder = new LateBinder(obj);
var retval = (int)lateBinder.Call("MethodName", arg1, arg2);
```

### "Reinventing the wheel"

There are no less than 50 packages about reflection & private members accessing.

- 🔍 https://www.nuget.org/packages?q=Tags%3A%22reflection%22

But I couldn't find any packages with my favorite syntax and features :).

So I decided to "reinvent the wheel" by my hand.


### Performance issue

In this library, `DynamicBinder` and `LateBinder` may be much slower because their implementation uses the reflection API directory without any technics such as caches, compiling to delegations, compiling to expressions, etc.

Therefore, I think there is plenty of room for improvement to faster, more high performance.

If you prefer, you can fork this repository and improve it.

## Release Notes

Release notes are [here.](https://github.com/jsakamoto/dynamicbinder/blob/master/RELEASE-NOTES.txt)

## Licence

- [GNU Lesser General Public License v3.0 or later](https://github.com/jsakamoto/dynamicbinder/blob/master/LICENSE)
