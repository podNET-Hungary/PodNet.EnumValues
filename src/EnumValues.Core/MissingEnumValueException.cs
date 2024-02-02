namespace PodNet.EnumValues;

/// <summary>Thrown by the generated extension methods when <see cref="ValuesAttribute{TValue}"/> is configured on an enum type to throw for missing annotations (<see cref="MissingValueHandling.ThrowMissingValueException"/>) or undefined enum values (<see cref="UndefinedValueHandling.ThrowMissingValueException"/>).</summary>
/// <param name="enumType">The enum <see cref="Type"/> that triggered the error.</param>
/// <param name="rawValue">The value that triggered the error. Can be missing or undefined based on the corresponding <see cref="ValuesAttribute{TValue}"/> configuration.</param>
public sealed class MissingEnumValueException(Type enumType, object rawValue) : Exception($"Missing enum value {rawValue} on type {enumType}.")
{
    public Type EnumType { get; } = enumType;
    public object RawValue { get; } = rawValue;
};
