using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace PodNet.EnumValues.CodeAnalysis;

public record AttributeArgumentsLookup(ImmutableDictionary<string, TypedConstant> Values)
{
    public static AttributeArgumentsLookup FromAttributeData(AttributeData attributeData)
    {
        if (attributeData.NamedArguments.Length is 0)
            return Empty;
        var result = new Dictionary<string, TypedConstant>(attributeData.NamedArguments.Length);
        foreach (var kv in attributeData.NamedArguments)
            result[kv.Key] = kv.Value;
        return new(result.ToImmutableDictionary());
    }

    public static AttributeArgumentsLookup Empty { get; } = new(ImmutableDictionary<string, TypedConstant>.Empty);
    public TypedConstant this[string key] => Values.TryGetValue(key, out var value) ? value : default;

    public T GetValueOrDefault<T>(string key, T defaultValue = default!)
    {
        if (Values.TryGetValue(key, out var value))
            return (T?)value.Value ?? defaultValue;
        return defaultValue;
    }
}
