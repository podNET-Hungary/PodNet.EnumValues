namespace PodNet.EnumValues.Text;

internal static class CodeText
{
    public static string TrimAttributeName(string attributeName)
        => attributeName.EndsWith("Attribute")
            ? attributeName[..^"Attribute".Length]
            : attributeName;

    public static string AlterCasing(string identifier, MissingValueHandling missingValueHandling)
    {
        if (missingValueHandling is not MissingValueHandling.PascalCasing and not MissingValueHandling.CamelCasing and not MissingValueHandling.KebabCasing and not MissingValueHandling.SnakeCasing)
            throw new InvalidOperationException($"Invalid {nameof(MissingValueHandling)} value: {missingValueHandling}");

        if (identifier is null or [])
            throw new ArgumentException("Invalid identifier: the identifier was null or empty.", nameof(identifier));

        Span<char> s = stackalloc char[identifier.Length * 2];
        var div = missingValueHandling switch
        {
            MissingValueHandling.KebabCasing => '-',
            MissingValueHandling.SnakeCasing => '_',
            _ => default
        };
        s[0] = missingValueHandling is MissingValueHandling.PascalCasing ? char.ToUpperInvariant(identifier[0]) : char.ToLowerInvariant(identifier[0]);
        var si = 0;
        for (var i = 1; i < identifier.Length; (i, si) = (i + 1, si + 1))
        {
            var current = identifier[i];
            if (!char.IsUpper(current) || missingValueHandling is MissingValueHandling.CamelCasing or MissingValueHandling.PascalCasing)
                s[si] = current;
            else
            {
                s[si] = char.ToLowerInvariant(current);
                s[++si] = div;
            }
        }
        return new(s[..si].ToArray());
    }
}
