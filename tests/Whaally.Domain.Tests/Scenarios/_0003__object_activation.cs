using Orleans.Configuration;

namespace Whaally.Domain.Tests.Scenarios;

public class _0003__object_activation
{
    public class ObjectWithoutConstructor;
    public class ObjectWithConstructor
    {
        public ObjectWithConstructor(object value)
        {
            
        }
    }

    [Fact]
    public void CanCreateObject() => ObjectHelpers.CreateCtor(typeof(ObjectWithoutConstructor));

    [Fact]
    public void CannotCreateObjectWithoutParameterlessConstructor() =>
        Assert.Throws<ArgumentNullException>(() => ObjectHelpers.CreateCtor(typeof(ObjectWithConstructor)));
}