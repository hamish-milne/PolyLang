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

`bool` and `null` values can only be compared with themselves, the exception being that `null == ""`.

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

Type hints can be combined additively with `,`. A variable with multiple types can be assigned a value that is compatible with at least one of them.

Of particular note are the `null` and `notnull` types; these will allow and prevent the `null` value, respectively.

If a variable is hinted as being `int`, `float`, `string` or `bool`, the `notnull` hint is implicit and as such the default assignment values will be `0`, `0.0`, `""` and `false` respectively. To allow null values simply include the `null` hint, in which case this will be the default.

Combining `notnull` with a type that is _not_ one of the four above (i.e. nothing, `array` or `object`) results in a value that _must_ be assigned as it is declared.

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
var bar : float, null = foo as float;   # bar is now null; no exception is thrown
```

## Functions

Poly supports two styles for function definitions:

```python
# Normal (full body) syntax
func foo(a, b, c = 5)      # c defaults to 5
    var d = a*b;
    return d + c;
end

# Expression syntax - returns the result of a single statement
func bar(a, b, c = 5) => a*b + c

# Function arguments and return values can have type hints:
func foo(a : float, b : int, c : int = 5) : float => a*b + c
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

Members have two access points that can be restricted: `read` and `write`. If no access point is specified, the default is to apply the keyword to both equally.

```python
var myObj = {
    private var foo;
    write:private var bar;
    read:public write:protected var baz;
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

