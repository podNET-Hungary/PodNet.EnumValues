namespace PodNet.EnumValues;

/// <summary>Defines what should happen when an undefined enum value is encountered (often resulting from unsanitized/invalid user input or an invalid cast from a raw value). Not to be confused with <see cref="MissingValueHandling"/>, which handles values that are defined, but have no associated <see cref="ValueAttribute"/>.</summary>
public enum UndefinedValueHandling
{
    /// <summary>The raw value itself is returned by calling <see cref="object.ToString"/> on the value.</summary>
    RawValueToString,
    /// <summary>Returns an empty string. This is useful if you want to implement a wrapper around the generated method and handle this case in user code.</summary>
    EmptyString,
    /// <summary>Throws a <see cref="MissingEnumValueException"/>.</summary>
    ThrowMissingValueException
}