using CpymoEditor.Core.Tools;

namespace CpymoEditor.Tests.Tools;

public sealed class ToolResultTests
{
    [Fact]
    public void Success_CreatesSuccessfulResultWithGeneratedFilesAndLogs()
    {
        ToolResult result = ToolResult.Success(
            "image-convert",
            new[] { "out/hero.png" },
            new[] { "Converted hero_mask.png to alpha PNG." });

        Assert.True(result.Succeeded);
        Assert.Equal("image-convert", result.Operation);
        Assert.Equal(new[] { "out/hero.png" }, result.GeneratedFiles);
        Assert.Empty(result.Problems);
        Assert.Contains("Converted", result.LogLines[0]);
    }

    [Fact]
    public void Failure_CreatesFailedResultWithProblem()
    {
        ToolResult result = ToolResult.Failure(
            "asset-analysis",
            new ToolProblem(ToolProblemSeverity.Error, "Missing background BG001_H.", "script/start.txt", 12));

        Assert.False(result.Succeeded);
        Assert.Single(result.Problems);
        Assert.Equal(ToolProblemSeverity.Error, result.Problems[0].Severity);
        Assert.Equal("script/start.txt", result.Problems[0].File);
        Assert.Equal(12, result.Problems[0].Line);
    }
}
