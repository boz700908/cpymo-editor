namespace CpymoEditor.Core.Tools;

public sealed record ToolResult(
    string Operation,
    bool Succeeded,
    IReadOnlyList<ToolProblem> Problems,
    IReadOnlyList<string> GeneratedFiles,
    IReadOnlyList<string> LogLines)
{
    public static ToolResult Success(
        string operation,
        IEnumerable<string>? generatedFiles = null,
        IEnumerable<string>? logLines = null)
    {
        return new ToolResult(
            operation,
            Succeeded: true,
            Array.Empty<ToolProblem>(),
            (generatedFiles ?? Array.Empty<string>()).ToArray(),
            (logLines ?? Array.Empty<string>()).ToArray());
    }

    public static ToolResult Failure(string operation, params ToolProblem[] problems)
    {
        return new ToolResult(
            operation,
            Succeeded: false,
            problems,
            Array.Empty<string>(),
            Array.Empty<string>());
    }
}
