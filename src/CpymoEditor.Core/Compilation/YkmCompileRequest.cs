namespace CpymoEditor.Core.Compilation;

public sealed record YkmCompileRequest(
    string SourcePath,
    string OutputDirectory,
    IReadOnlyList<string> LibraryPaths);
