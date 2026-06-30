namespace CpymoEditor.Core.Tools;

public sealed class CpymoToolCommandService : ICpymoToolService
{
    private static readonly HashSet<string> SupportedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png",
        ".jpg",
        ".jpeg",
        ".bmp"
    };

    private readonly string _executablePath;
    private readonly IToolProcessRunner _runner;

    public CpymoToolCommandService(string executablePath, IToolProcessRunner runner)
    {
        _executablePath = executablePath;
        _runner = runner;
    }

    public Task<ToolResult> AnalyzeAssetsAsync(string projectDirectory, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(projectDirectory))
        {
            return Task.FromResult(ToolResult.Failure(
                "asset-analysis",
                new ToolProblem(ToolProblemSeverity.Error, "Project directory does not exist.", projectDirectory)));
        }

        return Task.FromResult(ToolResult.Success(
            "asset-analysis",
            logLines: new[] { "Asset analysis command is reserved for the CPyMO asset analyzer integration." }));
    }

    public async Task<ToolResult> ConvertImagesAsync(string inputDirectory, string outputDirectory, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(inputDirectory))
        {
            return ToolResult.Failure(
                "image-convert",
                new ToolProblem(ToolProblemSeverity.Error, "Input directory does not exist.", inputDirectory));
        }

        Directory.CreateDirectory(outputDirectory);

        var generated = new List<string>();
        var logs = new List<string>();
        foreach (string inputFile in Directory.EnumerateFiles(inputDirectory))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!SupportedImageExtensions.Contains(Path.GetExtension(inputFile)))
            {
                continue;
            }

            string outputFile = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(inputFile) + ".png");
            ToolProcessResult process = await _runner.RunAsync(
                _executablePath,
                new[] { "resize-image", inputFile, outputFile, "1", "1", "--out-format", "png" },
                workingDirectory: null,
                cancellationToken);

            if (process.ExitCode != 0)
            {
                return FailedProcess("image-convert", process);
            }

            generated.Add(outputFile);
            AddProcessOutput(logs, process);
        }

        return ToolResult.Success("image-convert", generated, logs);
    }

    public async Task<ToolResult> PackageProjectAsync(string projectDirectory, string outputFile, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(projectDirectory))
        {
            return ToolResult.Failure(
                "package",
                new ToolProblem(ToolProblemSeverity.Error, "Project directory does not exist.", projectDirectory));
        }

        string fileList = Path.Combine(Path.GetTempPath(), "cpymo-editor-pack-" + Guid.NewGuid().ToString("N") + ".txt");
        try
        {
            IEnumerable<string> files = Directory.EnumerateFiles(projectDirectory, "*", SearchOption.AllDirectories);
            await File.WriteAllLinesAsync(fileList, files, cancellationToken);

            ToolProcessResult process = await _runner.RunAsync(
                _executablePath,
                new[] { "pack", outputFile, "--file-list", fileList },
                projectDirectory,
                cancellationToken);

            if (process.ExitCode != 0)
            {
                return FailedProcess("package", process);
            }

            return ToolResult.Success("package", new[] { outputFile }, SplitOutput(process));
        }
        finally
        {
            if (File.Exists(fileList))
            {
                File.Delete(fileList);
            }
        }
    }

    public Task<ToolResult> ValidateGameConfigAsync(string gameConfigPath, CancellationToken cancellationToken)
    {
        if (!File.Exists(gameConfigPath))
        {
            return Task.FromResult(ToolResult.Failure(
                "gameconfig-validate",
                new ToolProblem(ToolProblemSeverity.Error, "gameconfig.txt does not exist.", gameConfigPath)));
        }

        string[] lines = File.ReadAllLines(gameConfigPath);
        bool hasScriptType = lines.Any(line => line.StartsWith("scripttype,", StringComparison.Ordinal));
        bool hasStartScript = lines.Any(line => line.StartsWith("startscript,", StringComparison.Ordinal));

        var problems = new List<ToolProblem>();
        if (!hasScriptType)
        {
            problems.Add(new ToolProblem(ToolProblemSeverity.Error, "Missing scripttype entry.", gameConfigPath));
        }

        if (!hasStartScript)
        {
            problems.Add(new ToolProblem(ToolProblemSeverity.Warning, "Missing startscript entry.", gameConfigPath));
        }

        return Task.FromResult(problems.Any(problem => problem.Severity == ToolProblemSeverity.Error)
            ? new ToolResult("gameconfig-validate", false, problems, Array.Empty<string>(), Array.Empty<string>())
            : new ToolResult("gameconfig-validate", true, problems, Array.Empty<string>(), new[] { "gameconfig.txt is valid." }));
    }

    private static ToolResult FailedProcess(string operation, ToolProcessResult process)
    {
        string message = string.IsNullOrWhiteSpace(process.StandardError)
            ? "CPyMO tool command failed."
            : process.StandardError.Trim();

        return ToolResult.Failure(operation, new ToolProblem(ToolProblemSeverity.Error, message));
    }

    private static void AddProcessOutput(List<string> logs, ToolProcessResult process)
    {
        logs.AddRange(SplitOutput(process));
    }

    private static IReadOnlyList<string> SplitOutput(ToolProcessResult process)
    {
        return (process.StandardOutput + Environment.NewLine + process.StandardError)
            .Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
