### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
PN1601 | Design | Warning | `EnumValuesGenerator`: raised when enum type marked with `ValuesAttribute<TValue>` contains members that are not annotated and `MissingValueHandling` is `ThrowMissingValueException`.
PN1602 | Design | Warning | `EnumValuesGenerator`: raised when an enum type marked with `ValuesAttribute<TValue>` is contained in a generic type structure.
PN1603 | Design | Warning | `EnumValuesGenerator`: raised when multiple enum members map to the same underlying constant value.
PN1604 | Design | Warning | `EnumValuesGenerator`: raised when an enum type marked as having flags has values in range that are not able to be represented by the defined bit flags. In other words, some binary enum values are missing that are smaller than the largest valued defined member.