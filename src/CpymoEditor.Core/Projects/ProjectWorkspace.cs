using CpymoEditor.Core.Assets;
using CpymoEditor.Core.Configuration;
using CpymoEditor.Core.Events;

namespace CpymoEditor.Core.Projects;

public sealed record ProjectWorkspace(
    string ProjectDirectory,
    ProjectKind Kind,
    bool CanEdit,
    IReadOnlyList<string> Messages,
    EventDocument? Events,
    AssetLibrary Assets,
    GameConfigDocument? Config)
{
    public static ProjectWorkspace Open(string projectDirectory)
    {
        ProjectClassificationResult classification = ProjectClassifier.Classify(projectDirectory);
        AssetLibrary assets = AssetLibraryScanner.Scan(projectDirectory);

        if (!classification.CanOpen)
        {
            return new ProjectWorkspace(
                projectDirectory,
                classification.Kind,
                CanEdit: false,
                classification.Reasons,
                Events: null,
                assets,
                Config: null);
        }

        GameConfigDocument? config = LoadConfig(projectDirectory);
        EventDocument? events = classification.Kind == ProjectKind.NativePymo
            ? LoadNativePymoEvents(projectDirectory, config)
            : null;

        return new ProjectWorkspace(
            projectDirectory,
            classification.Kind,
            CanEdit: true,
            classification.Reasons,
            events,
            assets,
            config);
    }

    private static GameConfigDocument? LoadConfig(string projectDirectory)
    {
        string gameConfigPath = Path.Combine(projectDirectory, "gameconfig.txt");
        return File.Exists(gameConfigPath)
            ? GameConfigDocument.Parse(File.ReadAllText(gameConfigPath))
            : null;
    }

    private static EventDocument? LoadNativePymoEvents(string projectDirectory, GameConfigDocument? config)
    {
        string startScript = config?.GetValue("startscript") ?? "start";
        string scriptPath = Path.Combine(projectDirectory, "script", startScript + ".txt");
        if (!File.Exists(scriptPath))
        {
            string? firstScript = Directory.Exists(Path.Combine(projectDirectory, "script"))
                ? Directory.EnumerateFiles(Path.Combine(projectDirectory, "script"), "*.txt", SearchOption.AllDirectories).FirstOrDefault()
                : null;

            scriptPath = firstScript ?? scriptPath;
        }

        return File.Exists(scriptPath)
            ? PymoScriptParser.Parse(Path.GetRelativePath(projectDirectory, scriptPath), File.ReadAllText(scriptPath))
            : null;
    }
}
