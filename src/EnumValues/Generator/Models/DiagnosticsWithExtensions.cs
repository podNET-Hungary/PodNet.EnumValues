using Microsoft.CodeAnalysis;
using PodNet.EnumValues.Equality;

namespace PodNet.EnumValues.Generator.Models;

public record DiagnosticsWithExtensions(
    EquatableArray<Diagnostic>? Diagnostics, 
    EquatableArray<ExtensionToGenerate>? Extensions);
