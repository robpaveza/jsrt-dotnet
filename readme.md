# jsrt-dotnet
A library for accessing the Chakra and ChakraCore 
[JavaScript Runtime hosting](https://github.com/Microsoft/ChakraCore/wiki/JavaScript-Runtime-%28JSRT%29-Reference) 
interface from the .NET Framework.  It is inspired by [jsrt-winrt](https://github.com/robpaveza/jsrt-winrt) but 
is not directly compatible.

*Why do I care?*  If you want to extend your .NET application, or allow your users to do
so, by writing some JavaScript at runtime, this allows you to do so without paying the 
cost of loading an HTML engine with it.  And, it's far more convenient than the programming
model supported by the HTML engine.  (Unless you're using HTML rendering within your app, 
in which case using the HTML engine is pretty efficient for that).

## What is special about jsrt-dotnet?

This project aims to create as seamless a bridge between your JavaScript and .NET code as 
possible.  .NET objects can be directly exposed to JavaScript, and JavaScript objects can 
be accessed from C# using `dynamic` or via normal early binding.  It also aims to be an 
accurate representation of the JavaScript type system from .NET.

## Getting started

As an example, let's create a simple host function called Echo:

```csharp
static JavaScriptValue Echo(JavaScriptEngine engine, bool construct, JavaScriptValue thisValue, IEnumerable<JavaScriptValue> arguments)
{
    string fmt = arguments.First().ToString();
    object[] args = (object[])arguments.Skip(1).ToArray();
    Console.WriteLine(fmt, args);
    return engine.UndefinedValue;
}
```

The function must return a `JavaScriptValue` (because all JavaScript functions return 
something - even if that something is `undefined`).  The parameters to the function 
represent the things that the JavaScript code is calling:

 - `engine` is an isolated collection of globals and code
 - `construct` indicates whether the function is being called with the `new` operator
 - `thisValue` is the ambient JavaScript `this` value (if one is available)
 - `arguments` are the remaining parameters actually passed in

The function expects at least a single parameter to be passed, and will accept multiple
other parameters.  It then uses the `Console.WriteLine(string, params object[])` overload
to write a formatted string.  We then add this function to the global object:

```csharp
using (var runtime = new JavaScriptRuntime())
using (var engine = runtime.CreateEngine())
using (var context = engine.AcquireContext())
{
    engine.SetGlobalFunction("echo", Echo);

    // TODO: Call echo
}
```

So what we do here are:
 - Create a `JavaScriptRuntime`, which has global settings and a shared memory allocator
 - Create a `JavaScriptEngine`, which is that isolated collection of globals and code
 - Acquire an execution context from the engine, which means that the script engine is 
   bound to the current thread while the context is held.  The engine is released from 
   the thread when the context is disposed.
 - We then create a global function, `echo`, corresponding to the `Echo` method earlier

All that's left is to run some script that will call it:

```csharp
using (var runtime = new JavaScriptRuntime())
using (var engine = runtime.CreateEngine())
using (var context = engine.AcquireContext())
{
    engine.SetGlobalFunction("echo", Echo);

	var fn = engine.EvaluateScriptText(@"(function() {    
	echo('{0}, {1}!', 'Hello', 'World');
})();");
    fn.Invoke(Enumerable.Empty<JavaScriptValue>());
}
```

If you run this from a console, you'll see

    Hello, World!

output on the screen.  

## Using dynamic

Let's start getting crazy.

```csharp
using (var runtime = new JavaScriptRuntime())
using (var engine = runtime.CreateEngine())
using (var context = engine.AcquireContext())
{
    engine.SetGlobalFunction("echo", Echo);
    dynamic global = engine.GlobalObject;
    global.hello = "Hello";
    global.world = "world";

	var fn = engine.EvaluateScriptText(@"(function() {    
        echo('{0}, {1}!', hello, world);
    })();");
    fn.Invoke(Enumerable.Empty<JavaScriptValue>());
}
```

What happened here?

The `dynamic` keyword instructs the C# compiler to perform runtime late binding
on things that are performed against the dynamic thing.  The engine's 
`GlobalObject` property is a JavaScript `Object` (it's the thing that provides 
all of those handy things like `ArrayBuffer` and `Math`).  That Object, like 
any other JavaScript object, has properties, and those properties are accessible
via the [normal name resolution rules](http://es5.github.io/#x10.2) per normal
JavaScript semantics.

Because JavaScript Objects are property bags, we can assign anything to them. 
What happens under the covers is:

 1. `JavaScriptObject` derives from `JavaScriptValue`, which in turn derives
    from `DynamicObject`, provided in the .NET base class library
 2. The C# language bindings call `JavaScriptObject.TrySetMember`, passing 
    information about the operation, namely, that the name is `hello` and 
    that the set-member operation is case-sensitive.  (Because JavaScript 
    is case-sensitive, we ignore this flag, and always treat it as 
    case-sensitive).
 3. That operation converts the right-hand side value (in this case, a C# 
    `string` of `"Hello"`) to its JavaScript equivalent, a `JavaScriptValue`,
    and calls `JavaScriptObject.SetPropertyByName(string, JavaScriptValue)`
    method.
 4. When the script text is executed, the environment record has bindings for
    `hello` and `world` as properties of the global, so they're passed back 
    out to C# as `JavaScriptValue`s in the arguments.  They get converted 
    back to strings, without changing the code.

I can blow your mind even more.  Without even calling script, I can call into
the script engine:

```csharp
using (var runtime = new JavaScriptRuntime())
using (var engine = runtime.CreateEngine())
using (var context = engine.AcquireContext())
{
    engine.SetGlobalFunction("echo", Echo);
    dynamic global = engine.GlobalObject;
    
    global.echo("{0}, {1}, from dynamic.", "Hello", "world");
}
```

Here, the C# dynamic binder calls into `JavaScriptObject.TryInvokeMember`, 
which resolves the property, casts it to a `JavaScriptFunction`, and then 
calls it.  That function happens to be a host function, so it calls back
into C#, using the same round-trip behaviors as shown previously.

## Accessing CLR objects from script

CLR objects can be added to and accessed via script.  Wherever possible,
we try to preserve object behavioral semantics across the script-host
boundary.  That is, if you have a C# object that you add to the script
engine, and then mutate that object from script, those changes will be 
reflected in the C# object.

** Important: ** _.NET objects to JavaScript are still an incomplete and 
experimental feature.  The following outlines how this feature is intended
to function, but may not be implemented as described._

To convert a host object to a JavaScript representation, call 
`myJavaScriptEngine.Converter.FromObject`, which will return a 
`JavaScriptValue`.  

    var pt = new Point3D { X = 18, Y = 27, Z = -1 };
    engine.SetGlobalVariable("pt", engine.Converter.FromObject(pt));

For any type that isn't just represented by a JavaScript primitive, we 
attempt to follow this algorithm:

 - If the Type of the value is a `struct` (`System.ValueType`), an 
   `ArgumentException` is thrown.  Because struct types can define methods
   and properties, but object identity isn't preserved, they are invalid
   types for projection across the boundary.  Instead, use a JSON 
   serializer to serialize the value, and then deserialize the value on 
   the JavaScript side.
 - If the Type of the value is a `delegate` (derived from `System.Delegate`), 
   an `ArgumentException` is thrown.  Instead, use an overload of 
   `engine.CreateFunction`.
 - If the Type of the value is a `Task`, a `Promise` is created.  The 
   Promise will be resolved or rejected once the Task has completed.
 - Otherwise, an object will be projected as follows:
   - Get the Type of the object being converted
   - Create the constructor function:
    - If the Type defines one or more public constructors, create a function
      named the full name of the Type.  Only one overload of each arity can
      be supported.  The function, when called with or without the `new` 
      operator, will call the constructor.
    - If the Type does not define any public constructors, the function will
      still be created, but it will only return `undefined`.
    - Regardless of whether there are any constructors, the function will 
      not be added to the global namespace.  It will only be accessible via
      a `constructor` property.
   - Create the prototype object:
    - If the prototype object's base Type is not `null` (in other words, 
      if the Type isn't `System.Object`), create a prototype chain and 
      recycle this algorithm.
    - For each public method defined by this type, create a function that 
      trampolines from JavaScript into .NET based on number of arguments
      passed from JavaScript, and add it to the prototype object.
    - For each public property defined by this type, create an accessor 
      property on the prototype object, with get and/or set methods, which
      trampolines from JavaScript into .NET.  Indexer properties are not 
      supported.
    - If the type defines any events, create an `addEventListener` and 
      `removeEventListener` method.  If there are events in the type's 
      inheritance chain, the first thing that these methods should do is
      invoke the parent prototype's corresponding add/remove method.  The
      `addEventListener` method should add an event handler which, when 
      called, converts the .NET parameters into a single object.  The 
      object passed into the JavaScript callback has one property for 
      each named argument in the event delegate signature.  These 
      arguments are converted by .NET-to-JavaScript semantics.
    - For each event, an `on{eventname}` property will also be created.
      When the property is set, it will unregister any registered 
      listeners, and set a new listener (or not if set to a non-Function).
   - Public static properties, methods, and events will be projected in 
     the same way as instance properties, methods, and events; except 
     that events will not call via the prototype chain (type-bound events 
     belong to the type, and do not work against a prototype chain).


In addition to providing these via objects sent into the engine directly, the
developer can add a constructor to the global namespace via the 
`AddTypeToGlobal` function:

```csharp
engine.AddTypeToGlobal<Point3D>();
```

Instead of providing an instance of an object to the engine, this example 
creates a function named `Point3D` on the global object, representing the 
`Point3D` public constructors.

### Example

Given the following type definition:

```csharp
public class Point
{
    public double X
    {
        get;
        set;
    }
    
    public double Y
    {
        get;
        set;
    }
    
    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}

public class Point3D : Point
{
    public double Z
    {
        get;
        set;
    }
    
    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }
}
```

If the `Point3D` type is added to the global object, the equivalent code
is executed:

```js
(function(global, createPoint, getX, setX, getY, setY, pointToString, createPoint3D, getZ, setZ, point3DToString) {
	function Point() {
		if (!(this instanceof Point))
			return new Point(arguments);

		createPoint.call(this);
	}
	Object.defineProperty(Point.prototype, 'X', {
		get: getX,
		set: setX, 
		enumerable: true
	});
	Object.defineProperty(Point.prototype, 'Y', {
		get: getY, 
		set: setY,
		enumerable: true
	});
	Point.prototype.toString = pointToString;

	function Point3D() {
		if (!(this instanceof Point3D))
			return new Point3D(arguments);

		createPoint3D.call(this);
	}
	Point3D.prototype = Object.create(Point.prototype);
	Point3D.prototype.constructor = Point3D;
	Object.defineProperty(Point3D.prototype, 'Z', {
		get: getZ,
		set: setZ,
		enumerable: true
	});
	Point3D.prototype.toString = point3DToString;

	global.Point3D = Point3D;
})([native method representations]);
```

The work to project `Point` in this example is preserved, and reused, so 
if `Point3D` is added to the global before `Point`, adding `Point` will 
only need to add the identifier `Point` to the global object; the 
initialization of the prototype properties doesn't need to occur.
