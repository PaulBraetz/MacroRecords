using TestNamespace;

[MacroRecord]
[Field(typeof(ITestInterface), "Field")]
readonly partial struct MyClass { }

namespace TestNamespace
{
    interface ITestInterface
    {
        public String Value { get; set; }
    }
}