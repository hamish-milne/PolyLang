# Poly
## Subtitle goes here

In this project, I aim to create a language that combines rapid prototyping, easy interpretation and full language flexibility with optional strong typing and the ability to be compiled into high-performance bytecode.

## Basics

```
var foo               # Declare a dynamically typed variable
foo = 1               # Assign a value
foo = foo + "5"       # The normal '+' operator requires numbers.
                      # The string "5" is automatically converted into one
foo = foo + "five"    # If a value cannot be appropriately converted, an *exception* is thrown.
print(foo)            # A few functions are in the global namespace, such as `print`

# Every statement must be followed by either a new line or a semicolon:
foo += 1; foo += 2
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

```
123 == 123.456;      # 123 is converted to 123.0
"123" == 123.456;    # "123" is converted to 123.0
"123" == 123;        # "123" is converted to 123
"foo" == 123;        # "foo" cannot be converted; the comparison returns `false`
```

This applies to all comparisons:
* `==` Equality
* `!=` Inequality
* `>` Greater than
* `<` Less than
* `>=` Greater than or equal
* `<=` Less than or equal

When converting strings to numbers, Poly tends to be lenient, and will ignore whitespace before, as well as non-numeric characters after the number:

```
# This is a valid assignment
var a : float = "     123.456abcdef";

# This is not; the first non-whitespace character is not numeric
#var b : float = "abcd  123.456";
```

Poly has no strict equality operator, but it's very simple to emulate one:

```
if a is float and a == 123.456: ...

# Or, in a more general case..
if typeof(a) == typeof(b) and a == b: ...
```

`bool` and `null` values can only be compared with themselves, the exception being that `null == ""`.

All other comparisons will return `false`.

## Variables

Variables are declared with the `var` keyword, followed by an identifier for their name. They can be assigned in the same statement in the usual manner; the default initialization value is `null`.

By default, a variable is untyped, which means it can accept any value. Optionally, a variable can have a 'type hint', which restricts its type, causing an exception to be thrown if an improper value is assigned. Automatic casting will still apply, however.

Type hints can be combined additively with `,`. A variable with multiple types can be assigned a value that is compatible with at least one of them.

Of particular note are the `null` and `notnull` types; these will allow and prevent the `null` value, respectively.

If a variable is hinted as being `int`, `float`, `string` or `bool`, the `notnull` hint is implicit and as such the default assignment values will be `0`, `0.0`, `""` and `false` respectively. To allow null values simply include the `null` hint, in which case this will be the default.

Combining `notnull` with a type that is _not_ one of the four above (i.e. nothing, `array` or `object`) results in a value that _must_ be assigned as it is declared.

```
var foo;            # Untyped valua
var bar : float;    # Accepts floats - integers automatically cast
                    # No initial value; defaults to '0'
bar = "123.456";    # This is valid; string cast to float
#bar = "abc";       # This is not; "abc" cannot be converted
#var baz : notnull; # This isn't valid; we must assign the variable as we declare it
```
