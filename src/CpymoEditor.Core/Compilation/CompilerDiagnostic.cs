namespace CpymoEditor.Core.Compilation;

public sealed record CompilerDiagnostic(
    CompilerDiagnosticSeverity Severity,
    string? File,
    int? Line,
    int? Column,
    string Message);
