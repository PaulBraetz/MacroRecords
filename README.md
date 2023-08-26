# MacroRecords

*Create detailed C# record types without the boilerplate.*

## Feature Overview

Addressing some of what I believe to be are shortcomings of C# record types, these are the features `MacroRecords` support:

You may:
- configure the generated constructors visibility
- configure which fields shall be included in the debugger display 
- supply your own constructor(s)
  
You may define each fields:
- name
- type
- visibility
- availability to deconstruction
- support for the `withX(T x)` syntax
- inclusion in the debugger display
- inclusion in validation 

The source code generated (depending on your definitions) includes:
- a constructor
- `DebuggerDisplayAttribute`
- static methods `Create`, `TryCreate`, `IsValid`
- methods `WithX(x value)`
- a `Deconstruct(out x value1, out y value2)`
- an `implicit` conversion operator
- overrides for `Equals(object)`, `GetHashCode()`
- implementations for `Equals(T)`, `==`, `!=`

## Installation

*All releases are marked as prerelease until I am confident in their robustness.*

In order to use `MacroRecords`, install the source generator from its Nuget repository: [RhoMicro.MacroRecords](https://www.nuget.org/packages/RhoMicro.MacroRecords/).

The attributes required for instructing the source generator may be found in their own Nuget repository: [RhoMicro.MacroRecords.Attributes](https://www.nuget.org/packages/RhoMicro.MacroRecords/)

## Feature Examples

TODO

The `MacroRecords` source generator works by generating a partial type declaration 

### Using `MacroRecordAttribute`

The ``MacroRecordAttribute` is the primary 
```cs
```

## TODO

- better installation instructions
- fix attribute compilation clash

Credits for the name MacroRecords go to [knight](https://github.com/muhamedkarajic).
