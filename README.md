# ValueObjectGenerator

"Primitive obsession is the tendency to represent domain concepts as language primitives."

**Escape primitive obsession by introducing bespoke value object types for these concepts.**

This source generator will generate simple structs that wrap a single type.
The advantages to this approach include:
- type safety when operating on domain concepts
- value integrity checks upon instance creation and casting
- use of the ubiquitous language for programmatic domain representations

To get started, take a look at the `TestApp` project. It includes a sample value object named `Age`.

Note: 

Currently, there seems to be a problem with detecting attribute usage while using a `using` statement.
For the time being, the fully qualified attribute name `RhoMicro.ValueObjectGenerator.GeneratedValueObjectAttribute`
or `RhoMicro.ValueObjectGenerator.GeneratedValueObject` must be used in order for the generator to detectits usage. See the `TestApp.Program.cs` file for an example.