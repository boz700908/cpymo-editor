namespace CpymoEditor.Core.Assets;

public static class AssetLibraryScanner
{
    private static readonly IReadOnlyDictionary<string, AssetKind> KnownDirectories =
        new Dictionary<string, AssetKind>(StringComparer.OrdinalIgnoreCase)
        {
            ["bg"] = AssetKind.Background,
            ["chara"] = AssetKind.Character,
            ["bgm"] = AssetKind.Bgm,
            ["se"] = AssetKind.SoundEffect,
            ["voice"] = AssetKind.Voice,
            ["video"] = AssetKind.Video,
            ["system"] = AssetKind.SystemImage,
            ["script"] = AssetKind.Script
        };

    public static AssetLibrary Scan(string projectDirectory)
    {
        if (!Directory.Exists(projectDirectory))
        {
            return new AssetLibrary(projectDirectory, Array.Empty<AssetItem>());
        }

        var assets = new List<AssetItem>();
        foreach ((string directory, AssetKind kind) in KnownDirectories)
        {
            string fullDirectory = Path.Combine(projectDirectory, directory);
            if (!Directory.Exists(fullDirectory))
            {
                continue;
            }

            foreach (string file in Directory.EnumerateFiles(fullDirectory, "*", SearchOption.AllDirectories))
            {
                string relativePath = Path.GetRelativePath(projectDirectory, file);
                assets.Add(new AssetItem(
                    kind,
                    Path.GetFileNameWithoutExtension(file),
                    Path.GetExtension(file),
                    relativePath,
                    file));
            }
        }

        return new AssetLibrary(projectDirectory, assets);
    }
}
