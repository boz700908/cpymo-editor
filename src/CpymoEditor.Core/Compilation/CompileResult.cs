namespace CpymoEditor.Core.Compilation;

public sealed record CompileResult(
    bool Succeeded,
    IReadOnlyList<string> GeneratedFiles,
    IReadOnlyList<CompilerDiagnostic> Diagnostics,
    IReadOnlyList<string> LogLines);
