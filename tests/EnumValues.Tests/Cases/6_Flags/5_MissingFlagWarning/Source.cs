[Flags, Values<ValueAttribute>]
public enum MyEnum 
{ 
    [Value("0")] None = 0, 
    // One = 1 should be missing
    [Value("2")] Two = 2 
}