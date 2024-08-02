[Flags] // 👈 Either add the [Flags]...
[Values<ValueAttribute>(
    IsFlags = true, // ...or set IsFlags = true
    FlagsSeparator = "" // The default is " | ", which represents a binary OR operation (the default in .NET's default Enum.ToString() as well).
)]
public enum Permissions
{
    [Value("!")]  Invalid = -1,                   // 👈 You *can* supply any non-flag values, even negative values, as normal. Undefined negative values will throw an ArgumentOutOfRangeException, however.
    [Value("-")]  None = 0,
    [Value("R")]  Read = 1,
    [Value("W")]  Write = 2,
                  ReadWrite = Read | Write,       // 👈 You don't need to supply the value if you use shorthands...
    [Value("X")]  Execute = 4,
    [Value("RX")] ReadExecute = Read | Execute,   // 👈 ...but you CAN, if you want. The provided value is only used if you don't supply your own separator.
                  WriteExecute = Write | Execute, // 👈 You don't have to define all possible flag combinations, but it's shown here for completeness.
    [Value("A")]  All = Read | Write | Execute    // 👈 You can override the output with any custom values you like.
}
