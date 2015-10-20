# Formal syntax specification

## Identifiers

An identifier can be any combination of unicode letter and digit characters, and underscores. The first character must not be an arabic digit (`0-9`).

## Literals

* Decimal integer `1234`
* Decimal floating point `123.456`
* Scientific notation `123.456e-100` or `123.456E-100`
* Hexadecimal integer `0xABCD` or `0XABCD`
* Octal integer `0c0123` or `0C0123`
* Binary integer `0b01011` or `0B01011`
* String `"abcdef"`

## Expressions

An expression 'root' is either a literal value or a variable name (optionally a member of an object, accessed with `.`).

Subsequent expressions can be unary `<op><expr>`, binary `<expr><op><expr>`, function calls `<func>(<args...>)` and assignments `<variable>=<expr>`.

A statement can only be an assignment, function call, control flow or definition.

## Statement endings

A semicolon specifies a 'hard' end of statement. Its presence will cause the preceding statement to be evaluated. A newline (any of `\n`, `\r`, or `\r\n`) is a 'soft' end of statement. It won't cause the statement to be evaluated if the preceding token is a binary operator (any of the arithmetic, comparison, bitwise or logical operators), *or* the parser is within nested parentheses `()`.

## Variables

`var <name> [ : <type> [, <type>]] [ = <initializer>]`

Variable types must be a `type` object, but are evaluated dynamically like every other expression. If a type is invalid, an exception is thrown in the constructor. The initializer must be an expression.

## Functions

Functions can be defined two ways

`func <name>(<args...>) <body> end`

`func <name>(<args...>) => <expression>`

Anonymous functions are similar:

`func(<args...>) <body> end`

`func(<args...>) => expression`
