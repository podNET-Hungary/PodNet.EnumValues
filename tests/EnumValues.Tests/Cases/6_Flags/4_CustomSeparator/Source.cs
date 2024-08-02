﻿[Values<ValueAttribute>(IsFlags = true, FlagsSeparator = "|")]
public enum Permissions
{
    [Value("-")]  None = 0,
    [Value("R")]  Read = 1,
    [Value("W")]  Write = 2,
    [Value("X")]  Execute = 4
}
