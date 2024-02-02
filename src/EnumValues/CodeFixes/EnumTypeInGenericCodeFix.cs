using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PodNet.EnumValues.Generator;
using System.Collections.Immutable;
using System.Composition;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace PodNet.EnumValues.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumTypeInGenericCodeFix)), Shared]
public sealed class EnumTypeInGenericCodeFix : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create([EnumValuesGenerator.EnumTypesInGenericsNotSupportedDescriptor.Id]);

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        if (await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false) is not { } root)
            return;

        var diagnostic = context.Diagnostics[0];
        if (root.FindToken(diagnostic.Location.SourceSpan.Start).Parent is not EnumDeclarationSyntax enumTypeDeclaration)
            return;

        context.RegisterCodeFix(
            CodeAction.Create(diagnostic.GetMessage(), RemoveEnumTypeValuesAttributesAsync, diagnostic.Id),
            diagnostic);

        async Task<Solution> RemoveEnumTypeValuesAttributesAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var model = await context.Document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            return context.Document.WithSyntaxRoot(
                root.ReplaceNode(
                    enumTypeDeclaration,
                    enumTypeDeclaration.WithAttributeLists(
                        List(
                            enumTypeDeclaration.AttributeLists
                                .Select(al => AttributeList(SeparatedList(al.Attributes.Where(a => (model.GetSymbolInfo(a).Symbol is ITypeSymbol type) && type.ToString() == s_valuesAttributeMetadataName))))
                                .Where(a => a.Attributes.Count > 0)
                            )
                        )
                    ))
                .Project.Solution;
        }

    }

    private static readonly string s_valuesAttributeMetadataName = typeof(ValuesAttribute<>).FullName;
}
