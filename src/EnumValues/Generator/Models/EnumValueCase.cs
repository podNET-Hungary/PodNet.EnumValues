using Microsoft.CodeAnalysis;
using PodNet.EnumValues.Text;
using System.Collections.Immutable;
using static PodNet.EnumValues.Integrals.IntegralValues;

namespace PodNet.EnumValues.Generator.Models;

public sealed record EnumValueCase(
    string Identifier,
    string? Value,
    object? ConstantValue)
{
    public static ImmutableArray<EnumValueCase> CreateMany(INamedTypeSymbol enumType, MissingValueHandling missingValueHandling, ITypeSymbol valueMarkerAttributeType, bool isFlags, DiagnosticContainer diagnosticContainer)
    {
        var groups = enumType.GetMembers().Where(m => m.Kind == SymbolKind.Field).OfType<IFieldSymbol>().OrderBy(f => f.ConstantValue).GroupBy(f => f.ConstantValue);

        foreach (var group in groups.Where(g => g.Count() > 1))
        {
            var allLocations = group.SelectMany(v => v.Locations).ToArray().AsSpan();
            diagnosticContainer.Add(Diagnostic.Create(EnumValuesGenerator.RemoveDuplicateEnumValueDescriptor, allLocations[0], allLocations[1..].ToArray(), string.Join(", ", group.Select(v => v.Name)), enumType.Name));
        }

        var values = groups.Select(g => g.First()).ToList();

        if (isFlags)
        {
            var allPositiveFlags = new HashSet<ulong>();
            var maximumValue = 0UL;
            foreach (var value in values)
            {
                var c = value.ConstantValue?.ToString();
                if (c is null || c is ['-', ..])
                    continue;
                if (ulong.TryParse(c, out var u))
                {
                    if (u > maximumValue)
                        maximumValue = u;
                    allPositiveFlags.Add(u);
                }
                else
                    throw new InvalidOperationException($"Unrecognized format of constant value: {c}");
            }

            for (var i = 0UL; i < maximumValue; i = i == 0 ? 1 : i * 2)
            {
                if (!allPositiveFlags.Contains(i))
                    diagnosticContainer.Add(Diagnostic.Create(EnumValuesGenerator.UndefinedEnumFlagMemberDescriptor, enumType.Locations.FirstOrDefault(), i, $"0x{i:X}", enumType.Name));
            }
        }

        return values.Select(symbol =>
        {
            var value = symbol.GetAttributes().SingleOrDefault(a => a.AttributeClass?.ToString() == valueMarkerAttributeType.ToString())?.ConstructorArguments.FirstOrDefault().Value?.ToString()
                ?? missingValueHandling switch
                {
                    MissingValueHandling.ThrowMissingValueException => null,
                    MissingValueHandling.ToString => symbol.Name,
                    MissingValueHandling.RawValueToString => symbol.ConstantValue?.ToString() ?? "",
                    MissingValueHandling.EmptyString => "",
                    _ => CodeText.AlterCasing(symbol.Name, missingValueHandling)
                };

            var isFlagCandidate = IsFlagCandidate(symbol.ConstantValue);

            if (value is null && (!isFlags || isFlagCandidate))
                diagnosticContainer.Add(Diagnostic.Create(EnumValuesGenerator.MissingEnumValueDescriptor, symbol.Locations.FirstOrDefault(), ImmutableDictionary.CreateRange<string, string?>([new("attribute", valueMarkerAttributeType.Name)]), valueMarkerAttributeType.Name, symbol.ContainingSymbol.Name, symbol.Name));

            if (value is not null || !isFlags || isFlagCandidate)
                return new EnumValueCase(symbol.Name, value, symbol.ConstantValue);
            return null;
        }).Where(v => v is not null).ToImmutableArray()!;
    }
};