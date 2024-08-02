using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PodNet.Analyzers.CodeAnalysis;
using PodNet.EnumValues.Generator;
using System.Collections.Immutable;
using System.Composition;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace PodNet.EnumValues.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MissingEnumValueCodeFix)), Shared]
public sealed class MissingEnumValueCodeFix : CodeFixProvider
{
    public sealed override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create([EnumValuesGenerator.MissingEnumValueDescriptor.Id]);

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        if (await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false) is not { } root)
            return;

        if (root.FindToken(context.Diagnostics[0].Location.SourceSpan.Start).Parent is not EnumMemberDeclarationSyntax enumMemberDeclaration)
            return;

        if (enumMemberDeclaration.Parent is not EnumDeclarationSyntax enumDeclaration)
            return;


        foreach (var diagnostic in context.Diagnostics)
        {
            if (diagnostic.Properties["attribute"] is not { Length: > 0 } attribute)
                continue;

            context.RegisterCodeFix(CodeAction.Create(diagnostic.GetMessage(), CreateChangedSolution, $"{diagnostic.Id}+{attribute}"), diagnostic);

            Task<Solution> CreateChangedSolution(CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var newAttributeList = AttributeList(SingletonSeparatedList(
                    Attribute(IdentifierName(TextProcessing.TrimAttributeSuffix(attribute)),
                        AttributeArgumentList(SingletonSeparatedList(AttributeArgument(
                            LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(enumMemberDeclaration.Identifier.Text))))))));

                return Task.FromResult(context.Document.WithSyntaxRoot(root.ReplaceNode(enumMemberDeclaration, enumMemberDeclaration.AddAttributeLists(newAttributeList))).Project.Solution);
            }
        }
    }
}
