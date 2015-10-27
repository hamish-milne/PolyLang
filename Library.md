# Standard library

All of Poly's standard types and functions are contained within the namespace `poly.*`.

## Collections

Poly has a few collection types built in:

`poly.set` represents an unordered collection of unique values. Adding, removing and checking an item in a set is usually very quick to do. Sets can have bitwise operators applied to them to perform mathematical set-based operations.

```python
import poly.set

# The constructor accepts any number of arguments to initialize the set
var foo = set()
# The add operator similarly accepts multiple arguments
foo.add(5)
foo.add(unpack [3, 6, "bar", null, 6]) # '6' is added only once
# `add` returns the number of items successfully added, in this case '4'

# We can remove items in a similar way
foo.remove("bar", 3)

if foo.contains(6) then print("Success!") end

var bar = set()
# Remember that adding a set _won't_ add all the elements of that set..
bar.add(foo)

# To do that, use the union operator (or the `union` member function)
var baz = foo | bar

# The inversion operator (or `invert`) will return a set which
# is the inverse of its argument. `~set()` will appear to contain _every_
# possible element until elements are _removed_.
baz = ~foo

if baz.contains("unknown string") then print("Success!") end
```

`poly.map` is a dictionary type that allows a set of values to be mapped to additional data. Unlike `set` there is no simple 'add' operation; instead values are mapped using the index operator.

```python
import poly.map

var foo = map()
foo["abc"] = 1
foo[3] = null
# The 'add' function exists, but accepts a 'tuple'
import poly.tuple
foo.add(tuple("def", "ghi"), tuple(null, 8))
```
