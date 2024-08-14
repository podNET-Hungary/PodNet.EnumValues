using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Scripting;
using PodNet.Analyzers.Testing;
using PodNet.EnumValues.Generator;

namespace PodNet.EnumValues.Tests;

public class EnumValuesTestCase<T>(List<DiagnosticDescriptor>? expectedDescriptorsForGenerator = null, bool ignoreSources = false) : EmbeddedTestCase<EnumValuesGenerator, T>
{
    public EnumValuesTestCase(bool ignoreSources) : this(null, ignoreSources) { }
    public override bool IgnoreSources => ignoreSources;
    public override List<DiagnosticDescriptor>? ExpectedDescriptorsForGenerator => expectedDescriptorsForGenerator;
    public override CSharpCompilation CreateCompilation(IEnumerable<(string Name, string Content)> sources)
        => base.CreateCompilation(sources)
               .AddSyntaxTrees([CSharpSyntaxTree.ParseText("global using PodNet.EnumValues;")]);

    public override ScriptOptions ConfigureScriptOptions(ScriptOptions options)
        => base.ConfigureScriptOptions(options)
               .AddReferences("PodNet.EnumValues.Core")
               .AddImports("PodNet.EnumValues");

    // Need to duplicate the overrides with no viable multiple inheritance solution
    public new class WithCodeFix<TCodeFix>(List<DiagnosticDescriptor>? expectedDescriptorsForGenerator = null, bool ignoreSources = false) : EmbeddedTestCase<EnumValuesGenerator, T>.WithCodeFix<TCodeFix>
        where TCodeFix : CodeFixProvider, new()
    {
        public WithCodeFix(bool ignoreSources) : this(null, ignoreSources) { }
        public override bool IgnoreSources => ignoreSources;
        public override List<DiagnosticDescriptor>? ExpectedDescriptorsForGenerator => expectedDescriptorsForGenerator;
        public override CSharpCompilation CreateCompilation(IEnumerable<(string Name, string Content)> sources)
            => base.CreateCompilation(sources)
                   .AddSyntaxTrees([CSharpSyntaxTree.ParseText("global using PodNet.EnumValues;")]);

        public override ScriptOptions ConfigureScriptOptions(ScriptOptions options)
            => base.ConfigureScriptOptions(options)
                   .AddReferences("PodNet.EnumValues.Core")
                   .AddImports("PodNet.EnumValues");
    }
}
