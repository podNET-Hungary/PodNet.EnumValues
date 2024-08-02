public class MyClass<T> where T : class
{
    [Values<ValueAttribute>]
    public enum MyEnum
    {
        [Value("Member")]
        Member
    }
}