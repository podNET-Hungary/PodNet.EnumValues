using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using PodNet.Analyzers.Testing.CSharp;
using PodNet.EnumValues.Generator;
using System.Reflection;
using File = (string Name, string Content);

namespace PodNet.EnumValues.Tests;

public class EnumValuesTestCase<T>(IEnumerable<DiagnosticDescriptor>? expectedDescriptorsForGenerator = null, bool ignoreSources = false)
{
    protected List<DiagnosticDescriptor>? ExpectedDescriptorsForGenerator { get; } = expectedDescriptorsForGenerator?.ToList();
    protected bool IgnoreSources { get; } = ignoreSources;

    protected static IIncrementalGenerator[] Generators { get; } = [new EnumValuesGenerator()];
    public virtual IReadOnlyCollection<File> GetStaticPropertyValues(string nameSuffix) => typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static).Where(p => p.Name.EndsWith(nameSuffix)).Select(p => (p.Name, p.GetValue(null)!.ToString()!)).ToList().AsReadOnly();
    public virtual IReadOnlyCollection<File> GetSources() => GetStaticPropertyValues("Source_cs");
    public virtual IReadOnlyCollection<File> GetExpectedGeneratedSources() => GetStaticPropertyValues("Generated_cs");
    public virtual IReadOnlyCollection<File> GetScripts() => GetStaticPropertyValues("Script_cs");

    [TestMethod]
    public virtual async Task GeneratedCodeMatchesAndWorksAsExpectedAsync()
    {
        var sources = GetSources();
        var result = RunGenerator(sources, out var outputCompilation);
        AssertGenerationResults(result, outputCompilation);
        await AssertGeneratedBehaviorAsync(outputCompilation);
    }

    public virtual GeneratorDriverRunResult RunGenerator(IEnumerable<File> sources, out CSharpCompilation outputCompilation)
    {
        var compilation = PodCSharpCompilation.Create(["global using PodNet.EnumValues;"]).AddSyntaxTrees(sources.Select(s => CSharpSyntaxTree.ParseText(s.Content, path: s.Name)));
        return compilation.RunGenerators(Generators, out _, out outputCompilation);
    }

    public virtual void AssertGenerationResults(GeneratorDriverRunResult result, CSharpCompilation outputCompilation)
    {
        AssertDiagnostics(result, outputCompilation);

        if (IgnoreSources)
            return;

        var expectedSources = GetExpectedGeneratedSources();
        Assert.AreEqual(result.GeneratedTrees.Length, expectedSources.Count, $"Expected {expectedSources.Count} generated sources, actual: {result.GeneratedTrees.Length}");

        foreach (var (name, expectedSource) in expectedSources)
        {
            var expectedSyntax = CSharpSyntaxTree.ParseText(expectedSource);
            if (result.GeneratedTrees.Length == 1)
            {
                // We already checked for both to contain a single one, so expectedSource is the single that should match the single generated tree here
                Assert.IsTrue(SyntaxFactory.AreEquivalent(expectedSyntax, result.GeneratedTrees[0], false), $"The generated tree isn't syntactically equivalent to the expected source {name}.\r\nExpected:\r\n{expectedSource}\r\nActual:\r\n{result.GeneratedTrees[0]}");
            }
            else
            {
                // Find the matching one, if any
                Assert.IsTrue(result.GeneratedTrees.Any(g => SyntaxFactory.AreEquivalent(expectedSyntax, g, false)), $"None of the {result.GeneratedTrees.Length} generated trees matched the expected syntax of '{typeof(T).Name}.{name}'.");
            }
        }
    }

    public virtual void AssertDiagnostics(GeneratorDriverRunResult result, CSharpCompilation outputCompilation)
    {
        AssertGenerationRunResultDiagnostics(result);
        AssertCompilationDiagnostics(outputCompilation);
    }

    public virtual void AssertGenerationRunResultDiagnostics(GeneratorDriverRunResult result)
    {
        if (ExpectedDescriptorsForGenerator is { Count: > 0 })
        {
            CollectionAssert.AreEquivalent(ExpectedDescriptorsForGenerator, result.Diagnostics.Select(d => d.Descriptor).ToList());
        }
        else
            AssertNoDiagnostics(result.Diagnostics, "Expected no diagnostics from the generator.");

    }

    public virtual void AssertCompilationDiagnostics(CSharpCompilation compilation)
    {
        AssertNoDiagnostics(compilation.GetDiagnostics().Where(d => d.Severity is DiagnosticSeverity.Warning or DiagnosticSeverity.Error), "Expected no diagnostics of Warning or Error from the resulting compilation.");
    }

    protected static void AssertNoDiagnostics(IEnumerable<Diagnostic> diagnostics, string message)
    {
        Assert.AreEqual(0, diagnostics.TryGetNonEnumeratedCount(out var count) ? count : diagnostics.Count(), $"{message}\r\n{string.Join("\r\n", diagnostics.Select((d, i) => $"[{i}] {d.Severity} {d.Id} {d.GetMessage()}"))}");
    }

    public virtual async Task AssertGeneratedBehaviorAsync(CSharpCompilation outputCompilation)
    {
        var scripts = GetScripts();
        foreach (var (name, script) in scripts)
        {
            var scriptRunResult = await outputCompilation.ExecuteScriptAsync<object?>(script, configureOptions: o => o
                .AddReferences("Microsoft.VisualStudio.TestPlatform.TestFramework", "netstandard", "PodNet.EnumValues.Core")
                .AddImports("Microsoft.VisualStudio.TestTools.UnitTesting", "PodNet.EnumValues"));
            Assert.IsTrue(scriptRunResult.EmitResult.Success, $"Expected script '{typeof(T).Name}.{name}' to compile and emit correctly.");
            Assert.IsNull(scriptRunResult.ScriptResult, $"Expected script '{typeof(T).Name}.{name}' to return null.");
        }
    }

    public class WithCodeFix<TCodeFix>(IEnumerable<DiagnosticDescriptor>? expectedDescriptorsForGenerator = null) : EnumValuesTestCase<T>(expectedDescriptorsForGenerator)
        where TCodeFix : CodeFixProvider, new()
    {
        public virtual IReadOnlyCollection<File> GetFixedSources() => GetStaticPropertyValues("Source_Fixed_cs");

        [TestMethod]
        public virtual async Task CodeFixWorksAsExpectedAsync()
        {
            List<string> sourcesWithAppliedFixes = [];

            var codeFix = new TCodeFix();
            var result = RunGenerator(GetSources(), out var compilation);

            var project = new AdhocWorkspace().AddProject(codeFix.GetType().Name, LanguageNames.CSharp);
            foreach (var tree in compilation.SyntaxTrees)
                project = project.AddDocument(Path.GetFileName(tree.FilePath), tree.GetText(), filePath: tree.FilePath).Project;

            foreach (var diagnosticToFix in result.Diagnostics.Where(d => codeFix.FixableDiagnosticIds.Contains(d.Id)))
            {
                if (diagnosticToFix.Location.SourceTree is null)
                    throw new AssertFailedException("The required source tree was not available for extraction in the produced Diagnostic.");
                var sourceText = diagnosticToFix.Location.SourceTree.GetText().ToString();
                foreach (var document in project.Documents)
                {
                    var source = await document.GetTextAsync();
                    if (string.Equals(source.ToString(), sourceText))
                    {
                        var context = new CodeFixContext(document, diagnosticToFix, (codeAction, diagnostics) =>
                        {
                            var operations = codeAction.GetOperationsAsync(CancellationToken.None).GetAwaiter().GetResult();
                            foreach (var operation in operations)
                            {
                                if (operation is ApplyChangesOperation acOperation)
                                {
                                    var newDocument = acOperation.ChangedSolution.GetDocument(document.Id);
                                    // We find the updated document by the original's id
                                    Assert.IsNotNull(newDocument);
                                    // Collect the updated document's source content for assertion later on
                                    sourcesWithAppliedFixes.Add(newDocument.GetTextAsync().GetAwaiter().GetResult().ToString());
                                }
                                else
                                {
                                    // Should be no need to call operation.Apply here
                                    Assert.Fail("The operation recieved from the code fix wasn't of the expected type");
                                }
                            }
                        }, default);
                        await codeFix.RegisterCodeFixesAsync(context);
                        break;
                    }
                }
            }

            var expectedFixes = GetFixedSources();
            Assert.AreEqual(expectedFixes.Count, sourcesWithAppliedFixes.Count, $"The number of applied fixes ({sourcesWithAppliedFixes.Count}) didn't match the number of expected fixes ({expectedFixes.Count})");
            CollectionAssert.AreEquivalent(expectedFixes.Select(s => s.Content).Order().ToList(), sourcesWithAppliedFixes.Order().ToList(), "The applied code fixes didn't match the expected sources");
        }
    }
}
