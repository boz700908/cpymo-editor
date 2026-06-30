namespace CpymoEditor.Core.Tools;

public sealed record ToolProblem(
    ToolProblemSeverity Severity,
    string Message,
    string? File = null,
    int? Line = null);
