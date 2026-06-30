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
    public async Task ValidateGameConfigAsync_FailsWhenConfigFormatIsInvalid()
    {
        string gameConfig = Path.Combine(_root, "gameconfig.txt");
        await File.WriteAllTextAsync(gameConfig, "scripttype, pymo\n");
        var service = new CpymoToolCommandService("cpymo-tool", new RecordingToolProcessRunner(new ToolProcessResult(0, "", "")));

        ToolResult result = await service.ValidateGameConfigAsync(gameConfig, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Problems, problem => problem.Message.Contains("Spaces around comma", StringComparison.Ordinal));
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

    [Fact]
    public async Task AnalyzeAssetsAsync_ReportsMissingReferencedBackground()
    {
        string project = Path.Combine(_root, "game");
        Directory.CreateDirectory(Path.Combine(project, "script"));
        await File.WriteAllTextAsync(Path.Combine(project, "gameconfig.txt"), "scripttype,pymo\nbgformat,.jpg\nstartscript,start\n");
        await File.WriteAllTextAsync(Path.Combine(project, "script", "start.txt"), "#bg BG001_H\n");

        var service = new CpymoToolCommandService("cpymo-tool", new RecordingToolProcessRunner(new ToolProcessResult(0, "", "")));

        ToolResult result = await service.AnalyzeAssetsAsync(project, CancellationToken.None);

        Assert.False(result.Succeeded);
        ToolProblem problem = Assert.Single(result.Problems);
        Assert.Equal(ToolProblemSeverity.Error, problem.Severity);
        Assert.Equal(Path.Combine(project, "script", "start.txt"), problem.File);
        Assert.Equal(1, problem.Line);
        Assert.Contains("BG001_H", problem.Message);
    }

    [Fact]
    public async Task AnalyzeAssetsAsync_SucceedsWhenCommonReferencedAssetsExist()
    {
        string project = Path.Combine(_root, "game");
        Directory.CreateDirectory(Path.Combine(project, "script"));
        Directory.CreateDirectory(Path.Combine(project, "bg"));
        Directory.CreateDirectory(Path.Combine(project, "bgm"));
        Directory.CreateDirectory(Path.Combine(project, "chara"));
        Directory.CreateDirectory(Path.Combine(project, "se"));
        Directory.CreateDirectory(Path.Combine(project, "voice"));
        await File.WriteAllTextAsync(
            Path.Combine(project, "gameconfig.txt"),
            "scripttype,pymo\nbgformat,.jpg\nbgmformat,.mp3\ncharaformat,.png\nseformat,.wav\nvoiceformat,.mp3\nstartscript,start\n");
        await File.WriteAllTextAsync(
            Path.Combine(project, "script", "start.txt"),
            """
            #bg BG001_H
            #bgm BGM00
            #chara 0,AY04BA,50,0,300
            #se SE02
            #vo PRO000
            """);
        await File.WriteAllTextAsync(Path.Combine(project, "bg", "BG001_H.jpg"), "fake");
        await File.WriteAllTextAsync(Path.Combine(project, "bgm", "BGM00.mp3"), "fake");
        await File.WriteAllTextAsync(Path.Combine(project, "chara", "AY04BA.png"), "fake");
        await File.WriteAllTextAsync(Path.Combine(project, "se", "SE02.wav"), "fake");
        await File.WriteAllTextAsync(Path.Combine(project, "voice", "PRO000.mp3"), "fake");

        var service = new CpymoToolCommandService("cpymo-tool", new RecordingToolProcessRunner(new ToolProcessResult(0, "", "")));

        ToolResult result = await service.AnalyzeAssetsAsync(project, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Empty(result.Problems);
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
