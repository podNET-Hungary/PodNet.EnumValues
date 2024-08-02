using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PodNet.Analyzers;
using PodNet.Analyzers.CodeAnalysis;
using PodNet.EnumValues.Generator.Models;

namespace PodNet.EnumValues.Generator;

[Generator(LanguageNames.CSharp)]
public sealed class EnumValuesGenerator : IIncrementalGenerator
{
    public static readonly DiagnosticDescriptor MissingEnumValueDescriptor = new("PN1601", "Value attribute is missing for enum value", "Add [{0}] to the value '{1}.{2}' or set 'MissingValueHandling' to other than 'ThrowMissingValueException'", "Design", DiagnosticSeverity.Warning, true);
    public static readonly DiagnosticDescriptor EnumTypesInGenericsNotSupportedDescriptor = new("PN1602", "Enum types declared in generic types are not supported", $"Remove the [{TextProcessing.TrimAttributeSuffix(nameof(ValuesAttribute<ValueAttribute>))}] attribute(s) from {{0}}, or move it to a non-generic type or namespace", "Design", DiagnosticSeverity.Warning, true);
    public static readonly DiagnosticDescriptor RemoveDuplicateEnumValueDescriptor = new("PN1603", "Enum members with aliases can lead to unexpected results", "The members '{0}' in enum {1} represent the same values, which can lead to unexpected results. You should keep one member for every unique value.", "Design", DiagnosticSeverity.Warning, true);
    public static readonly DiagnosticDescriptor UndefinedEnumFlagMemberDescriptor = new("PN1604", "Undefined enum member for flag", "Add a member for the flag value {0} ({1}) to the enum type {2}", "Design", DiagnosticSeverity.Warning, true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var enumTypes = context.SyntaxProvider.ForAttributeWithMetadataName<DiagnosticsOrResults<ExtensionToGenerate>>(
            typeof(ValuesAttribute<>).FullName,
            static (node, _) => node is EnumDeclarationSyntax,
            static (context, cancellationToken) =>
            {
                var enumType = (INamedTypeSymbol)context.TargetSymbol;
                if (enumType.IsGenericType)
                    return Diagnostic.Create(EnumTypesInGenericsNotSupportedDescriptor, enumType.Locations[0], enumType.Name);

                var diagnosticContainer = new DiagnosticContainer();
                var results = ExtensionToGenerate.CreateMany(enumType, context.Attributes, diagnosticContainer, cancellationToken);
                return new(diagnosticContainer.ToEquatableArray(), results);
            });

        context.RegisterSourceOutputWithDiagnostics(enumTypes, (context, extension) =>
            context.AddSource(
                hintName: $"{$"{extension.EnumInfo.Identifier}_{extension.MethodName}".Replace('.', '_')}.g.cs",
                source: new ExtensionCodeBuilder(extension, context.CancellationToken).Build()));
    }
}
