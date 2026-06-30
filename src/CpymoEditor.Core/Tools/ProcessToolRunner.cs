using System.Diagnostics;

namespace CpymoEditor.Core.Tools;

public sealed class ProcessToolRunner : IToolProcessRunner
{
    public async Task<ToolProcessResult> RunAsync(
        string executablePath,
        IReadOnlyList<string> arguments,
        string? workingDirectory,
        CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = executablePath,
            WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        foreach (string argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Unable to start tool process.");
        string stdout = await process.StandardOutput.ReadToEndAsync(cancellationToken);
        string stderr = await process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);

        return new ToolProcessResult(process.ExitCode, stdout, stderr);
    }
}
