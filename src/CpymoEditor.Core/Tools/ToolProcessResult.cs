namespace CpymoEditor.Core.Tools;

public sealed record ToolProcessResult(
    int ExitCode,
    string StandardOutput,
    string StandardError);
