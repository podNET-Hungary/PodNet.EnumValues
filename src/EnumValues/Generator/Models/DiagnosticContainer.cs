using Microsoft.CodeAnalysis;
using PodNet.EnumValues.Equality;
using System.Collections.Immutable;

namespace PodNet.EnumValues.Generator.Models;

public sealed class DiagnosticContainer
{
    private readonly List<Diagnostic> _diagnostics = [];
    public void Add(Diagnostic diagnostic) => _diagnostics.Add(diagnostic);
    public EquatableArray<Diagnostic> ToEquatableArray() => _diagnostics.ToImmutableArray();
}
