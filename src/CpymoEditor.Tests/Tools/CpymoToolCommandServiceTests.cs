using CpymoEditor.Core.Tools;

namespace CpymoEditor.Tests.Tools;

public sealed class CpymoToolCommandServiceTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "cpymo-editor-tool-tests", Guid.NewGuid().ToString("N"));

    public CpymoToolCommandServiceTests()
    {
        Directory.CreateDirectory(_root);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }

    [Fact]
    public async Task PackageProjectAsync_CallsCpymoToolPackWithGeneratedFileList()
    {
        string project = Path.Combine(_root, "game");
        Directory.CreateDirectory(project);
        await File.WriteAllTextAsync(Path.Combine(project, "gameconfig.txt"), "scripttype,pymo\n");
        await File.WriteAllTextAsync(Path.Combine(project, "icon.png"), "fake");

        var runner = new RecordingToolProcessRunner(new ToolProcessResult(0, "packed", ""));
        var service = new CpymoToolCommandService("cpymo-tool", runner);

        ToolResult result = await service.PackageProjectAsync(project, Path.Combine(_root, "game.pak"), CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Single(runner.Calls);
        Assert.Equal("cpymo-tool", runner.Calls[0].ExecutablePath);
        Assert.Equal("pack", runner.Calls[0].Arguments[0]);
        Assert.Contains("--file-list", runner.Calls[0].Arguments);
        Assert.Contains("packed", result.LogLines);
    }

    [Fact]
    public async Task ValidateGameConfigAsync_FailsWhenRequiredFileIsMissing()
    {
        var service = new CpymoToolCommandService("cpymo-tool", new RecordingToolProcessRunner(new ToolProcessResult(0, "", "")));

        ToolResult result = await service.ValidateGameConfigAsync(Path.Combine(_root, "gameconfig.txt"), CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Problems, problem => problem.Severity == ToolProblemSeverity.Error);
    }

    [Fact]
    public async Task ConvertImagesAsync_ConvertsEverySupportedImageThroughResizeImage()
    {
        string input = Path.Combine(_root, "input");
        string output = Path.Combine(_root, "output");
        Directory.CreateDirectory(input);
        await File.WriteAllTextAsync(Path.Combine(input, "a.png"), "fake");
        await File.WriteAllTextAsync(Path.Combine(input, "b.txt"), "ignored");

        var runner = new RecordingToolProcessRunner(new ToolProcessResult(0, "converted", ""));
        var service = new CpymoToolCommandService("cpymo-tool", runner);

        ToolResult result = await service.ConvertImagesAsync(input, output, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Single(runner.Calls);
        Assert.Equal(new[] { "resize-image", Path.Combine(input, "a.png"), Path.Combine(output, "a.png"), "1", "1", "--out-format", "png" }, runner.Calls[0].Arguments);
    }

    private sealed class RecordingToolProcessRunner(ToolProcessResult result) : IToolProcessRunner
    {
        public List<ToolProcessCall> Calls { get; } = [];

        public Task<ToolProcessResult> RunAsync(string executablePath, IReadOnlyList<string> arguments, string? workingDirectory, CancellationToken cancellationToken)
        {
            Calls.Add(new ToolProcessCall(executablePath, arguments.ToArray(), workingDirectory));
            return Task.FromResult(result);
        }
    }

    private sealed record ToolProcessCall(string ExecutablePath, IReadOnlyList<string> Arguments, string? WorkingDirectory);
}
