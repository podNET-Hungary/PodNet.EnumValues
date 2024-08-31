namespace PodNet.EnumValues;

/// <summary>Defines the behavior that is executed when a defined enum value has no associated <see cref="ValueAttribute"/> defined to it.</summary>
public enum MissingValueHandling
{
    /// <summary>Throws a <see cref="MissingEnumValueException"/> if the enum value (or flag) has no value defined to it. This also enables an analyzer that warns if a defined enum value on the type has no corresponding annotation.</summary>
    ThrowMissingValueException,
    /// <summary>Returns the enum name as a string. Does not actually call <see cref="object.ToString()"/>, but rather returns the name as a constant string instance directly, which is faster.</summary>
    ToString,
    /// <summary>Returns the enum name as a <c>PascalCasedString</c> by swapping the first character to be uppercased.</summary>
    PascalCasing,
    /// <summary>Returns the enum name as a <c>camelCasedString</c>. Assumes the name is in PascalCase.</summary>
    CamelCasing,
    /// <summary>Returns the enum name as a <c>kebab-cased-string</c>. Assumes the name is in PascalCase.</summary>
    KebabCasing,
    /// <summary>Returns the enum name as a <c>snake_cased_string</c>. Assumes the name is in PascalCase.</summary>
    SnakeCasing,
    /// <summary>The raw constant value itself is returned by calling <see cref="object.ToString()"/> on the value.</summary>
    RawValueToString,
    /// <summary>Returns an empty string. This is useful if you want to implement a wrapper around the generated method and handle this case in user code.</summary>
    EmptyString,
    /// <summary>Returns the enum name lowercased (culture invariant).</summary>
    ToLowerInvariant,
    /// <summary>Returns the enum name uppercased (culture invariant).</summary>
    ToUpperInvariant
}