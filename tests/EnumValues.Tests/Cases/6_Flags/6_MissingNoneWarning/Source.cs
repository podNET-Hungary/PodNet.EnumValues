[Flags, Values<ValueAttribute>]
public enum MyEnum 
{ 
    // None = 0 should be missing
    [Value("1")] One = 1,
    [Value("2")] Two = 2 
}