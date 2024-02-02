namespace PodNet.EnumValues;

/// <summary>Marks the given enum type so that an extension method will be generated for it with a fast inline <c>switch</c> lookup for each of its members with a <see cref="ValueAttribute"/> (or descendant) attributes.
/// <code>
/// // You can use the provided ValueAttribute or derive from it.
/// public sealed class ColorAttribute(string value) : ValueAttribute(value);
/// 
/// [Values&lt;ColorAttribute&gt;]
/// public enum Sentiment
/// {
///     [Color("Green")]  Good,
///     [Color("Yellow")] Neutral,
///     [Color("Red")]    Bad
/// }
/// Console.WriteLine(Sentiment.Good.GetColor()); // Green
/// </code>
/// </summary>
/// <remarks>Note that you can reuse the same namespace, class and even method name for the generated extension method and class for different enum types, as they are overloadable based on the parameter, but the class visibility for different partial declarations has to match.</remarks>
/// <typeparam name="TValue">The type of attribute to look for on the enum values. Must be <see cref="ValueAttribute"/> or your custom descendant of it.</typeparam>
[AttributeUsage(AttributeTargets.Enum, AllowMultiple = true)]
public sealed class ValuesAttribute<TValue> : Attribute where TValue : ValueAttribute
{
    /// <summary>Namespace of the generated extension class. Default is to use the enum type's containing namespace.</summary>
    public string? Namespace { get; set; }
    /// <summary>Accessibility/visibility of the generated extension class and method. Default is to inherit the enum type's accessibility. If you reuse the same class for the generated extensions, the accessibilities have to match.</summary>
    public Accessibility Accessibility { get; set; }
    /// <summary>The extension class name. Default is <c>"{EnumTypeName}ValueExtensions"</c>.</summary>
    public string? ClassName { get; set; }
    /// <summary>The name of the generated extension method. Leave null for the default, which is <c>"Get{Attribute}"</c> (translates to <c>"GetValue"</c> if <see cref="TValue"/> is <see cref="ValueAttribute"/>). You can reuse the same partial class for different enum types (even with the same namespace, class, and method names), as they are overloadable if the enum types differ.</summary>
    public string? MethodName { get; set; }
    /// <summary>Defines what should happen when an undefined enum value is encountered (often resulting from unsanitized/invalid user input or an invalid cast from a raw value). Not to be confused with <see cref="MissingValueHandling"/>, which handles values that are defined, but have no associated <see cref="ValueAttribute"/>. The default is <see cref="UndefinedValueHandling.RawValueToString"/>.</summary>
    public UndefinedValueHandling UndefinedValueHandling { get; set; }
    /// <summary>Defines the behavior that is executed when a defined enum value has no associated <see cref="ValueAttribute"/> defined to it. The default is <see cref="MissingValueHandling.ThrowMissingValueException"/>, which also enables an analyzer that warns if an enum member hasn't defined the corresponding value with a <typeparamref name="TValue"/> typed attribute.</summary>
    public MissingValueHandling MissingValueHandling { get; set; }
    /// <summary>Describes if the decorated enum type represents flag (binary) values. The generator defaults to true if [<see cref="FlagsAttribute"/>] is also present on the type declaration.</summary>
    /// <remarks>When this evaluates to true, <see cref="MissingValueHandling"/> only takes into account values with a power of two and the 0 value, and <see cref="UndefinedValueHandling"/> pertains only to flag values that are larger than possible to represent using the given value (so, greater than or equal to twice the defined largest binary enum flag value).
    /// <para>When using flags, a single value of the enum will be represented in binary, and all binary (power of two) values where a given value evaluates to having the given flag set will be concatenated by the generator.</para></remarks>
    public bool IsFlags { get; set; }
    /// <summary>Override the default separator when getting the flag values of the type if <see cref="IsFlags"/> evaluates to true (being set manually or by the enum type having a [<see cref="FlagsAttribute"/>]); ignored otherwise. The default is <c>" | "</c>. You can supply a different separator individually when invoking the generated method.</summary>
    public string? FlagsSeparator { get; set; }
}
