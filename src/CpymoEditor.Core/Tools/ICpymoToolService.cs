namespace CpymoEditor.Core.Tools;

public interface ICpymoToolService
{
    Task<ToolResult> AnalyzeAssetsAsync(string projectDirectory, CancellationToken cancellationToken);

    Task<ToolResult> ConvertImagesAsync(string inputDirectory, string outputDirectory, CancellationToken cancellationToken);

    Task<ToolResult> PackageProjectAsync(string projectDirectory, string outputFile, CancellationToken cancellationToken);

    Task<ToolResult> ValidateGameConfigAsync(string gameConfigPath, CancellationToken cancellationToken);
}
