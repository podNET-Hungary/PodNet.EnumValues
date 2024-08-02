using Microsoft.CodeAnalysis;
using PodNet.Analyzers;
using PodNet.Analyzers.CodeAnalysis;
using PodNet.Analyzers.Equality;
using System.Collections.Immutable;

namespace PodNet.EnumValues.Generator.Models;

using V = ValuesAttribute<ValueAttribute>;

public sealed record ExtensionToGenerate(
    EnumInfo EnumInfo,
    string ValueMarkerAttributeTypeName,
    string? Namespace,
    Accessibility Accessibility,
    string ClassName,
    string MethodName,
    EquatableArray<EnumValueCase> ValueCases,
    MissingValueHandling MissingValueHandling,
    UndefinedValueHandling UndefinedValueHandling,
    bool IsFlags,
    string FlagsSeparator)
{
    public static ExtensionToGenerate Create(EnumInfo enumInfo, AttributeArgumentsLookup args, ITypeSymbol valueMarkerAttributeType, INamedTypeSymbol enumType, DiagnosticContainer diagnosticContainer, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var missingValueHandling = args.GetValue<MissingValueHandling>(nameof(V.MissingValueHandling));
        var methodName = args.GetValue<string?>(nameof(V.MethodName)) ?? $"Get{TextProcessing.TrimAttributeSuffix(valueMarkerAttributeType.Name)}";
        var isFlags = args.GetValue<bool?>(nameof(V.IsFlags)) ?? enumInfo.HasFlagsAttribute;

        return new ExtensionToGenerate(
            EnumInfo: enumInfo,
            ValueMarkerAttributeTypeName: valueMarkerAttributeType.Name,
            Namespace: args.GetValue<string?>(nameof(V.Namespace)) ?? enumInfo.Namespace,
            Accessibility: args.GetValue<Accessibility?>(nameof(V.Accessibility)) ?? (Accessibility)enumInfo.DeclaredAccessibility,
            ClassName: args.GetValue<string?>(nameof(V.ClassName)) ?? $"{enumInfo.Identifier.Replace('.', '_')}ValueExtensions",
            MethodName: methodName,
            ValueCases: EnumValueCase.CreateMany(enumType, missingValueHandling, valueMarkerAttributeType, isFlags, diagnosticContainer),
            MissingValueHandling: missingValueHandling,
            UndefinedValueHandling: args.GetValue<UndefinedValueHandling>(nameof(V.UndefinedValueHandling)),
            IsFlags: isFlags,
            FlagsSeparator: args.GetValue<string?>(nameof(V.FlagsSeparator)) ?? " | "
        );
    }

    public static ImmutableArray<ExtensionToGenerate> CreateMany(INamedTypeSymbol enumType, ImmutableArray<AttributeData> attributes, DiagnosticContainer diagnosticContainer, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var enumInfo = EnumInfo.Create(enumType);
        return attributes.Select(attributeData => Create(
                enumInfo,
                args: AttributeArgumentsLookup.FromAttributeData(attributeData),
                valueMarkerAttributeType: attributeData.AttributeClass?.TypeArguments.SingleOrDefault() ?? throw new InvalidOperationException("Couldn't identify the marker attribute."),
                enumType,
                diagnosticContainer,
                cancellationToken)).ToImmutableArray();
    }
};
