namespace CpymoEditor.Core.Tools;

using CpymoEditor.Core.Configuration;

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

        string gameConfigPath = Path.Combine(projectDirectory, "gameconfig.txt");
        if (!File.Exists(gameConfigPath))
        {
            return Task.FromResult(ToolResult.Failure(
                "asset-analysis",
                new ToolProblem(ToolProblemSeverity.Error, "gameconfig.txt does not exist.", gameConfigPath)));
        }

        string scriptDirectory = Path.Combine(projectDirectory, "script");
        if (!Directory.Exists(scriptDirectory))
        {
            return Task.FromResult(ToolResult.Failure(
                "asset-analysis",
                new ToolProblem(ToolProblemSeverity.Error, "script directory does not exist.", scriptDirectory)));
        }

        Dictionary<string, string> config = ReadGameConfig(gameConfigPath);
        var problems = new List<ToolProblem>();

        foreach (string scriptFile in Directory.EnumerateFiles(scriptDirectory, "*.txt", SearchOption.AllDirectories))
        {
            cancellationToken.ThrowIfCancellationRequested();
            int lineNumber = 0;
            foreach (string line in File.ReadLines(scriptFile))
            {
                lineNumber++;
                AnalyzeScriptLine(projectDirectory, config, scriptFile, lineNumber, line, problems);
            }
        }

        return Task.FromResult(problems.Count == 0
            ? ToolResult.Success("asset-analysis", logLines: new[] { "No missing common assets were found." })
            : new ToolResult("asset-analysis", false, problems, Array.Empty<string>(), Array.Empty<string>()));
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

        GameConfigDocument document;
        try
        {
            document = GameConfigDocument.Parse(File.ReadAllText(gameConfigPath));
        }
        catch (FormatException exception)
        {
            return Task.FromResult(ToolResult.Failure(
                "gameconfig-validate",
                new ToolProblem(ToolProblemSeverity.Error, exception.Message, gameConfigPath)));
        }

        bool hasScriptType = document.GetValue("scripttype") is not null;
        bool hasStartScript = document.GetValue("startscript") is not null;

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

    private static Dictionary<string, string> ReadGameConfig(string gameConfigPath)
    {
        var config = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (string line in File.ReadLines(gameConfigPath))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            int comma = line.IndexOf(',');
            if (comma <= 0)
            {
                continue;
            }

            config[line[..comma]] = line[(comma + 1)..];
        }

        return config;
    }

    private static void AnalyzeScriptLine(
        string projectDirectory,
        IReadOnlyDictionary<string, string> config,
        string scriptFile,
        int lineNumber,
        string line,
        List<ToolProblem> problems)
    {
        string trimmed = line.Trim();
        if (!trimmed.StartsWith('#'))
        {
            return;
        }

        string command = trimmed.Split([' ', '\t'], 2, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
        string argumentText = trimmed.Length > command.Length ? trimmed[command.Length..].Trim() : "";
        string[] arguments = argumentText.Split(',', StringSplitOptions.TrimEntries);

        switch (command)
        {
            case "#bg" when arguments.Length >= 1:
                AddMissingAssetProblem(projectDirectory, "bg", arguments[0], ConfigValue(config, "bgformat", ".jpg"), scriptFile, lineNumber, problems);
                break;
            case "#bgm" when arguments.Length >= 1:
                AddMissingAssetProblem(projectDirectory, "bgm", arguments[0], ConfigValue(config, "bgmformat", ".mp3"), scriptFile, lineNumber, problems);
                break;
            case "#se" when arguments.Length >= 1:
                AddMissingAssetProblem(projectDirectory, "se", arguments[0], ConfigValue(config, "seformat", ".wav"), scriptFile, lineNumber, problems);
                break;
            case "#vo" when arguments.Length >= 1:
                AddMissingAssetProblem(projectDirectory, "voice", arguments[0], ConfigValue(config, "voiceformat", ".mp3"), scriptFile, lineNumber, problems);
                break;
            case "#chara":
                AnalyzeChara(projectDirectory, config, scriptFile, lineNumber, arguments, problems);
                break;
        }
    }

    private static void AnalyzeChara(
        string projectDirectory,
        IReadOnlyDictionary<string, string> config,
        string scriptFile,
        int lineNumber,
        string[] arguments,
        List<ToolProblem> problems)
    {
        for (int index = 1; index < arguments.Length - 1; index += 4)
        {
            string assetName = arguments[index];
            if (string.Equals(assetName, "NULL", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            AddMissingAssetProblem(projectDirectory, "chara", assetName, ConfigValue(config, "charaformat", ".png"), scriptFile, lineNumber, problems);
        }
    }

    private static string ConfigValue(IReadOnlyDictionary<string, string> config, string key, string fallback)
    {
        return config.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : fallback;
    }

    private static void AddMissingAssetProblem(
        string projectDirectory,
        string directory,
        string assetName,
        string extension,
        string scriptFile,
        int lineNumber,
        List<ToolProblem> problems)
    {
        if (string.IsNullOrWhiteSpace(assetName))
        {
            return;
        }

        string path = Path.Combine(projectDirectory, directory, assetName + extension);
        if (File.Exists(path))
        {
            return;
        }

        problems.Add(new ToolProblem(
            ToolProblemSeverity.Error,
            $"Missing asset {assetName}{extension} in {directory}.",
            scriptFile,
            lineNumber));
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
