namespace PodNet.EnumValues;

/// <summary>Base attribute to use for decorating enum <b>values</b> for generating fast lookups for enum <b>types</b> decorated with <see cref="ValuesAttribute{TValue}"/>. 
/// <p>You can simply derive from this attribute to create another named marker attribute like so: <br/><c>public sealed class MyNameAttribute(string value) : ValueAttribute(value);</c><br/> 
/// This is especially useful when you:
/// <list type="bullet">
/// <item>want to define multiple lookups, for example, localization for different laguages like <c>EnAttribute</c> and <c>JpAttribute</c>,</item>
/// <item>want to specify or simplify consistent naming of the extension methods generated, for example a <c>NameAttribute</c> for <c>GetName</c> method(s) (see also <see cref="ValuesAttribute{TValue}.MethodName"/>).</item>
/// </list>
/// </p>
/// <p>
/// To generate the extension methods for an enum type: 
/// <list type="number">
/// <item>Decorate the enum type with <see cref="ValuesAttribute{TValue}"/>, and supply <see cref="ValueAttribute"/> or one of its descendants to its type parameter.</item>
/// <item>Use the type parameter supplied to <see cref="ValuesAttribute{TValue}"/> in step 1. to decorate the individual enum <b>values</b>.</item>
/// <item>Extension methods should automatically be generatedon the fly, as defined by the rules of <see cref="ValuesAttribute{TValue}"/> in step 1.</item>
/// </list>
/// </p>
/// </summary>
/// <param name="value">The value to assign to the decorated enum value.</param>
/// <typeparam name="TValue">The value type to asssign to the individual enum values.</typeparam>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class ValueAttribute(string value) : Attribute
{
    public string Value { get; set; } = value;
}
