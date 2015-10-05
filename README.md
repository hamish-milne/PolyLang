# Poly
## Subtitle goes here

In this project, I aim to create a language that combines rapid prototyping, easy interpretation and full language flexibility with optional strong typing and the ability to be compiled into high-performance bytecode.

## Basics

```
var foo; # Declare a dynamically typed variable
foo = 1; # Assign a value
foo = foo + "5"; # The normal '+' operator requires numbers.
                 # The string "5" is automatically converted into one
foo = foo + "five";  # If a value cannot be appropriately converted, an *exception* is thrown.
print(foo);  # A few functions are in the global namespace, such as `print`
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

`object`s and `array`s are, by default, passed by reference for performance reasons. `array`s are compared by value, however, whereas `object`s are compared by reference. Specialised functions allow you to pass either by value, and compare either for reference or value equality.

In comparisons, Poly follows a strict priority order for type conversions:
1. `float`
2. `int`
3. `string`

If one operand has a lower priority than the other, it will be converted as necessary.

```
123 == 123.456; # 123 is converted to 123.0
"123" == 123.456; # "123" is converted to 123.0
"123" == 123; # "123" is converted to 123
"foo" == 123; # "foo" cannot be converted; the comparison returns `false`
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
var a : float = "     123.456abcdef"; # This is a valid assignment
var b : float = "abcd  123.456"; # This is not; the first non-whitespace character is not numeric
```

Poly has no strict equality operator, but it's very simple to emulate one:

```
if a is float and a == 123.456: ...

# Or, in a more general case..
if typeof(a) == typeof(b) and a == b: ...
```

`bool` and `null` values can only be compared with themselves.

All other comparisons will return `false`.
