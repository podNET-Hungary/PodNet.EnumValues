using Microsoft.CodeAnalysis;

namespace PodNet.EnumValues.Generator.Models;

public sealed record EnumInfo(
    string? Namespace,
    Microsoft.CodeAnalysis.Accessibility DeclaredAccessibility,
    string Identifier,
    string UnderlyingType,
    bool HasFlagsAttribute)
{
    public bool CanBeNegative { get; } = UnderlyingType is "sbyte" or "short" or "int" or "long";

    public static EnumInfo Create(INamedTypeSymbol symbol)
    {
        var enumNamespace = symbol.ContainingNamespace is { IsGlobalNamespace: true } ? null : symbol.ContainingNamespace?.ToDisplayString();
        var enumAccessibility = symbol.DeclaredAccessibility;
        var containingTypeFullName = symbol.ContainingType?.ToDisplayString();
        var containingTypeRelativeName = containingTypeFullName is not null && enumNamespace is not null && containingTypeFullName.StartsWith(enumNamespace) ? containingTypeFullName[enumNamespace.Length..].TrimStart('.') : null;

        return new EnumInfo(
            Namespace: enumNamespace,
            DeclaredAccessibility: enumAccessibility,
            Identifier: !string.IsNullOrWhiteSpace(containingTypeRelativeName) ? $"{containingTypeRelativeName}.{symbol.Name}" : symbol.Name,
            UnderlyingType: symbol.EnumUnderlyingType?.ToDisplayString() ?? "int",
            symbol.GetAttributes().Any(a => a.AttributeClass?.ToString() == "System.FlagsAttribute"));
    }
}
