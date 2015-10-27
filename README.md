# Poly
## Subtitle goes here

In this project, I aim to create a language that combines rapid prototyping, easy interpretation and full language flexibility with optional strong typing and the ability to be compiled into high-performance bytecode.

Initial use case would be as a domain-specific language, which I believe it is tailored for, but it would be simple to convert into JavaScript, PHP, Python or Lua, or indeed to be run using a dedicated web server module.

My inspirations are Lua for the style and object functionality, JavaScript for some of the syntax, Python for some of the control flow, and C# for the object-oriented principles and encapsulation.

## Basics

```python
var foo               # Declare a dynamically typed variable
foo = 1               # Assign a value
foo = foo + "5"       # The normal '+' operator requires numbers.
                      # The string "5" is automatically converted into one
foo = foo + "five"    # If a value cannot be appropriately converted, an exception is thrown
print(foo)            # A few functions are in the global namespace, such as `print`

# Every statement must be followed by either a new line or a semicolon:
foo += 1; foo += 2

# FizzBuzz example...
for i : int in range(0, 100) do
    var printNewline : bool
    if i % 3 == 0 then
        print("Fizz", false)
        printNewLine = true
    end
    if i % 5 == 0 then
        print("Buzz", false);
        printNewLine = false;
    end
    if printNewLine then print("", true) end
end
```

## Types

Poly uses a lightweight dynamic typing system. It has only a few primitive types:

* `int`: An integer, at least 64-bits in maximum size.
* `float`: A double-precision floating point number.
* `string`: A string of UTF-8 characters.
* `bool`: `true` or `false`
* `object`: A user-defined object.
* `array`: A dynamically-expanding, ordered list of values.
* `null`: Used for no value, or uninitialized variables.

Poly's standard library contains further standard types, such as `set` and `map`, but these are all of the primitive type `object`.

_Objects_ and _arrays_ are, by default, passed by reference for performance reasons. _Arrays_ are compared by value, however, whereas _objects_ are compared by reference. Specialised functions allow you to pass either by value (by cloning), and compare either for reference or value equality.

Poly follows a strict priority order for type conversions:

1. `float`
2. `int`
3. `string`

If one operand has a lower priority than the other, it will be converted as necessary.

```python
123 == 123.456;      # 123 is converted to 123.0
"123" == 123.456;    # "123" is converted to 123.0
"123" == 123;        # "123" is converted to 123
"foo" != 123;        # "foo" cannot be converted
```

This applies to all comparisons and primitive operations.

When converting strings to numbers, Poly tends to be lenient, and will ignore whitespace before, as well as non-numeric characters after the number:

```python
# This is a valid assignment
var a : float = "     123.456abcdef";

# This is not; the first non-whitespace character is not numeric
#var b : float = "abcd  123.456";
```

`bool` and `null` values can only be compared with themselves, the exception being that `null == ""`. Additionally, `null` can be used as an operand in any arithmetic or bitwise operation, acting as a 'no-op'. So `1+null == 1`, `1/null == 1`, and `1*null == 1`.

Poly has no strict equality operator, but it's very simple to emulate one:

```python
if a is float and a == 123.456: ...

# Or, in a more general case..
if typeof(a) == typeof(b) and a == b: ...
```

However this generally isn't necessary, because the type conversions are very simple and well defined.

```python
# For example...
"  123.456" != "123.456 ";
123.456 != 123;
false != null;
# etc.
```

## Variables

Variables are declared with the `var` keyword, followed by an identifier for their name. They can be assigned in the same statement in the usual manner; the default initialization value is `null`.

By default, a variable is untyped, which means it can accept any value. Optionally, a variable can have a 'type hint', which restricts its type, causing an exception to be thrown if an improper value is assigned. Automatic casting will still apply, however.

Of particular note are the `nullable` and `notnull` types; these will allow and prevent the `null` value, respectively. They can be combined with other types using the `&` operator.

If a variable is hinted as being `int`, `float`, `string`, `bool` or `array`, the `notnull` hint is implicit and as such the default assignment values will be `0`, `0.0`, `""`, `false` and `[]` respectively. To allow null values simply include the `null` hint, in which case this will be the default.

Combining `notnull` with a type that is _not_ one of the four above (i.e. nothing or `object`) results in a value that _must_ be assigned as it is declared.

```python
var foo;             # Untyped value
var bar : float;     # Accepts floats - integers automatically cast
                     # No initial value; defaults to '0'
bar = "123.456";     # This is valid; string cast to float
#bar = "abc";        # This is not; "abc" cannot be converted
#var baz : notnull;  # This isn't valid; we must assign the variable as we declare it
```

Type hints aren't just limited to primitives; they can be *interfaces* and *function signatures* as well.

## Safe casting

Sometimes it's useful to test if a value has the right type, especially if you want to be strict about type correctness. While you can use the more verbose try-catch pattern, Poly also provides the `as` keyword, which will return `null` if the cast cannot be performed for any reason.

```python
var foo = "this is not a number";
var bar : float & nullable = foo as float;   # bar is now null; no exception is thrown
```

## Functions

Poly supports two styles for function definitions:

```python
# Normal (full body) syntax
func foo(a, b = 3, c = 5)      # c defaults to 5
    var d = a*b;
    return d + c;
end

# Expression syntax - returns the result of a single statement
func bar(a, b = 3, c = 5) => a*b + c

# Function arguments and return values can have type hints:
func foo(a : float, b : int, c : int = 5) : float => a*b + c

# Function calls with a single argument can be written two ways:
foo(2)
foo 2
```

Anonymous functions are simple; just omit the identifier

```python
var foo = func(a, b) => a + b
var bar = func(a, b) return a + b; end
```

Functions always have a return value. If there is no `return` statement, the function returns `null`.

Poly requires that all type-hinted arguments have a value, either as a default argument in the function declaration or when being called. Arguments with no type can be omitted, and are given a default value of `null`.

Varadic functions are defined by naming the final parameter `args` and optionally giving it an array type.

```python
func foo(a, b, args : int[])
    var c : int;
    for i in args do c += i end
    return c*a;
end

var bar = foo(3, "str", 1, 2, 3, 4, 5)
```

The `args` value follows similar default value rules; if it has a type hint, it always has a value (even if that value is an empty array), otherwise it may be `null`.

If an array is passed as a varadic parameter, it will be treated as a single value unless it is prepended with the `unpack` keyword.

```python
var myArgs = [1, 2, 3, 4, 5]

foo(3, "str", unpack myArgs)

# We can also use 'unpack' to populate non-varadic arguments:
foo(unpack myArgs)
```

## Passing by reference

Sometimes it's useful to pass values by reference, to allow the function to modify them as an output. This is accomplished by prepending the `ref` keyword to an argument.

```python
func foo(ref a) a = 5 end

var bar;
foo(ref bar);   # Note that 'ref' is required when we call the function as well
```

## Operators

The following operators can be defined by objects - see the relevant chapter for more information.

* Binary:
    * `+` - Addition
    * `-` - Subtraction
    * `*` - Multiplication
    * `/` - Division
    * `%` - Modulo/remainder
    * `.` - Concatenation
    * `&` - Bitwise AND, intersection
    * `|` - Bitwise OR, union
    * `^` - Bitwise XOR, exclusion
    * `in` - Checks presence in collection
* Unary:
    * `-` - Unary minus
    * `~` - Unary inversion, bitwise NOT
* Comparison:
    * `==` - Equality
    * `>` - Greater
    * `<` - Less
* Derived comparison (cannot be object-defined):
    * `!=` - Inequality (inverse of equality)
    * `>=` - Greater or equal (inverse of less)
    * `<=` - Less or equal (inverse of greater)

Additionally, Poly has several boolean-specific logical operators, which cannot be overloaded:

* `and`
* `or`
* `not`
* `xor`

## Conditionals

Conditionals use 'verbal' syntax, like Lua:

```python
if a and b then
    ...
else if c then
    ...
end

# Conditionals can have a 'return' value, making 'ternary' operations simple:
var foo = if a then 5 else 3
```

Unlike many other languages, when deciding whether to branch on a condition, Poly does not simply compare it to the boolean `true`. In fact it uses the special `op_condition` member function, which is much more lenient and makes for less boilerplate in code.

The default rules are:
* `bool` - no change
* `int` - `value != 0`

## Arrays

```python
var foo : [];           #  Array creation is implicit with constraints
var bar = [1, 2, 3, 4]; #  Filled arrays can be created
var baz : int[], null;  #  Array restricted to 'int' values

foo += "123";           #  Appending members is done with '+'
foo.add("456");         #  We can use a specific 'add' method as well

foo[5] = "abc";         #  Indexing is unrestricted
print(foo.count);       #  This prints '6', because we wrote to the 6th element before
print(foo[4]);          #  This prints 'null'
print(foo[10]);         #  This also prints 'null', even though it's outside the range
#print(foo[-1]);        #  This will throw an exception

baz = foo as int[];     #  baz is now 'null', because "abc" and 'null' can't be converted
baz = foo.ofType(int, 0);  #  baz is now [123, 456, 0, 0, 0, 0]
```

We can also do set-based operations with arrays:

```python
var foo = [1, 2, 3, 4];
var bar = [3, 4, 5, 6];

print(foo | bar);              # [1, 2, 3, 4, 3, 4, 5, 6]
print(foo & bar);              # [3, 4]
print(foo ^ bar);              # [1, 2, 5, 6]
print(4 in foo);               # true
print([1, 2] in foo);          # false; array treated as a single value
print(unpack[1, 2] in foo);    # true
```

Array splicing is done with `:`:

```python
var foo = [1, 2, 3, 4, 5, 6];

print(foo[1:])  # [2, 3, 4, 5, 6]
print(foo[:4])  # [1, 2, 3, 4]
print(foo[-2:]) # [5, 6]
print(foo[:-3]) # [1, 2, 3]
print(foo[1:3]) # [2, 3, 4]
```

In general, the first operand specifies the _starting index_ of the splice, which defaults to 0. The second operand is the _length_, which if omitted will add all remaining elements.

If the starting index is negative, the splice begins that many values from the _end_ rather than the beginning. Similarly, if the length is negative, the splice will end that many values _away_ from the end of the array.

All splice operations will return an array (assuming they are performed on an array), even if the operands are out of range. A simple way to check if a given operation is 'valid' is to confirm that `length - index >= array.count`, but if there are empty operands it becomes a bit more complicated. As such, you can use `array.safeSplice` which performs identically, but will throw an exception if the operands are out of range.

## Indefinite loops

Both 'while' and 'do-while' loops are supported:

```python
while condition do
    ...
end

do
    ...
while condition;
```

## For-loops and iterators

Any object that implements the `iterable` interface can be iterated over:

```python
interface iterable = {
    getIterator();
    moveNext(ref iterator) : bool;
    current(iterator);
}
```

We can then iterate over these objects like so:

```python
var container = ...

var it = container.getIterator()
while container.moveNext(ref it) do
    print(container.current(it))
end
```

However, we can simplify this using the `for` loop:

```python
var container = ...

for i in container do
    print(i)
end
```

## Objects

```python
var foo = {
    var bar;       # Member variable
    baz(a) => 5;   # Member function
    
    bar = 5;       # Statements in the object body are executed immediately
    print(bar);    # Parent members can be accessed without qualification
    
    # If a member is defined over a parent member, the latter can be accessed like so
    print(parent.bar)
}
```

Objects can be concatenated together, like so:

```python
var foo = {
    var a, b, c;
}

var derived = foo {
    var d, e, f;
}
```

In these 'derived' objects, functions can be reassigned:

```python
var foo {
    bar(a, b) => a + b
}

var derived = foo {
    # The 'base' keyword accesses the previous definition of the value
    bar(a, b) = base.bar(a, b)*b
}
```

Note that the syntax being used here will modify the _existing_ instance of the object. To ensure that this doesn't happen, you should use the 'constructor pattern' like so:

```python
func baseClass() => {
    var a, b, c;
}

func derived() => baseClass() {
    var d, e, f;
}

# Every call to 'derived' will create a new 'class->derived' instance
var myObj = derived();
```

Since inheritance happens on a per-object level, you can be entirely flexible with your class heirarchy at runtime.

## Interfaces

An interface defines a set of public members which can be used together as a type. Unlike most strongly typed languages, interfaces in Poly are inferred - a given object is checked dynamically, as needed.

To define an interface, simply assign an object that implements it:

```python
interface iterable = {
    getIterator();
    moveNext(ref iterator) : bool;
    current(iterator);
}
```

The object used in the interface definition is scanned for public functions and properties (variables are treated identically to a read-write property), which are then stored as a stub. The object reference won't be pinned as a result.

Interfaces can be combined together using the `&` operator. The result is a 'compound type'.

```python
interface foo = { func1(); }
interface bar = { func2(); }
interface baz = foo & bar;
# 'baz' defines 'func1' *and* 'func2'

# We can also join the interfaces in a variable declaration
var myVar : foo & bar;
```

## Properties

Properties are variables with a property accessor object assigned to them, which overrides the 'get' and 'set' operations to that variable. They must be defined with the keyword `prop`.

```python
var a;
prop foo = {
    get => a*2;
    set(value) => a = value/2;
}

foo = 5;   # a is now 2.5

# Changing a property accessor object can be done with the 'setvar' function:
setvar(ref foo, ...)   # This will only work in the same scope the property was defined in
```

Get-only properties can be defined with a single expression:

```python
prop foo => 1.23456789
```

## Access modifiers

To permit proper encapsulation, Poly allows members to be defined with access modifiers, which prevent (or allow) access from a different scope.

Three modifier keywords exist: `public` allows access from any scope, `private` disallows access from any scope but the one the member is defined in, and `protected` allows access to 'derived scopes' - ones which are appended to the object scope.

Note that `private` does not prevent access to _child_ scopes or objects, only to the _parent_ and _siblings_.

Members have two access points that can be restricted: `get` and `set`. If no access point is specified, the default is to apply the keyword to both equally.

```python
var myObj = {
    var private foo;
    var private:set bar;
    var public:get protected:set baz;
}

var myDerived = myObj {
    #foo = 5;    # Invalid; foo is private
    print(bar);
    #bar = 5;    # Invalid; bar is private
    baz = 5;     # Valid; we're in a derived object
}
```

By default, variables and properties are defined as `public`, and functions are defined as `read:public write:protected`. If only one access point is changed, the other remains at the default.

## Sealing

Variables can be 'sealed', preventing them from being written to in the future.

```python
var foo = 5;
seal foo;
#foo = 6;   # Invalid; foo is sealed

# Variables can be sealed as they are assigned:
var bar;
seal bar = 5;

# And even as they are declared
sealed baz = 5;
#sealed baz;   # This isn't valid; sealed variables must have an initializer

# Sealing can be overridden by using setvar
setvar(ref bar, 6);
```

## Modules

In Poly, every execution scope (file, interpreter instance etc.) is, itself, an object. This allows 'modules' to be 'imported' by simply executing code, and assigning the resultant object to a variable.

```python
import myModule

# We can rename the imported object, if it's more convenient
import someModule as anotherModule

# By default, accessing imported members is done via the object:
anotherModule.myFunction()
```

We can also import one or more members from the given module directly into our own scope.

```python
import foo.myFunc  # Imports only myFunc
myFunc()

import foo.*  # Imports all members of foo
# This is usually quite dangerous, as we don't know that 'foo'
# won't try to define a member that already exists in our scope.
```

One important thing to consider is that the _code_ in a given module is executed only once. The 'instance' of the module is kept in memory throughout the program's execution, with further general `import` calls doing nothing, and importing specific members will simply reference the existing instance. This makes importing cheap and predictable, but as a result means the 'state' (member variables) of an imported module will persist.

Additionally, the module variable is considered 'sealed' to user code. Member variables within the module, however, may not be. Access modifiers will still apply.

## Exceptions

Poly supports the handling of any exception that isn't a parse error (because with the latter, the state of the parser is invalid, so it's impossible to determine where the catch block is).

```python
var foo : int;
try
    foo = "not a number";
catch
    print("Error!");
end

# We can catch specific exceptions
try
    foo = "not a number";
catch(poly.invalidCast)
    print("Error!");
end

# .. And get the exception object for more information
try
    foo = "not a number";
catch(poly.invalidCast e)
    print(e.message);
end
```

One can create and throw custom exceptions too:

```python
func myException() => {
    toString() => "An error occurred"
}

func foo(int a)
    if a < 0 then throw myException(); end
end
```

## Type objects

Poly has a unified runtime and 'meta' type system. The keywords `int`, `float` and even `nullable` (but not `null`, which is an object) are in fact references to 'type objects', which can be used in type hints and casts, as well as being inspected as you would in a 'reflection' context.

In addition, type objects can be assigned to variables, and the resultant variable can be used in type hints and casts. This makes metaprogramming very simple:

```python
# The 'type' type constraint allows, as expected, only type objects
func myClass(t : type) => {
    var foo : t;
    var bar : t[];
}

myClass(int);
myClass(string);
```

## Operator overloading

Objects can specify custom behaviour for operators and casts...
