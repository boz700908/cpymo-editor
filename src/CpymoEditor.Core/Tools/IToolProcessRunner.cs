namespace CpymoEditor.Core.Tools;

public interface IToolProcessRunner
{
    Task<ToolProcessResult> RunAsync(
        string executablePath,
        IReadOnlyList<string> arguments,
        string? workingDirectory,
        CancellationToken cancellationToken);
}
