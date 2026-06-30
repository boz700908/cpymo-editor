using CpymoEditor.Core.Assets;

namespace CpymoEditor.Tests.Assets;

public sealed class AssetLibraryScannerTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "cpymo-editor-assets-tests", Guid.NewGuid().ToString("N"));

    public AssetLibraryScannerTests()
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
    public void Scan_GroupsKnownAssetDirectories()
    {
        CreateAsset("bg", "BG001_H.jpg");
        CreateAsset("chara", "AY04BA.png");
        CreateAsset("bgm", "BGM00.mp3");
        CreateAsset("unknown", "ignored.bin");

        AssetLibrary library = AssetLibraryScanner.Scan(_root);

        Assert.Equal(3, library.Assets.Count);
        Assert.Contains(library.Assets, asset => asset.Kind == AssetKind.Background && asset.Name == "BG001_H");
        Assert.Contains(library.Assets, asset => asset.Kind == AssetKind.Character && asset.Name == "AY04BA");
        Assert.Contains(library.Assets, asset => asset.Kind == AssetKind.Bgm && asset.Name == "BGM00");
        Assert.DoesNotContain(library.Assets, asset => asset.Name == "ignored");
    }

    [Fact]
    public void Scan_ReturnsRelativePathsForAssets()
    {
        CreateAsset("voice", "PRO000.mp3");

        AssetLibrary library = AssetLibraryScanner.Scan(_root);

        AssetItem asset = Assert.Single(library.Assets);
        Assert.Equal(Path.Combine("voice", "PRO000.mp3"), asset.RelativePath);
        Assert.Equal(Path.Combine(_root, "voice", "PRO000.mp3"), asset.FullPath);
    }

    private void CreateAsset(string directory, string fileName)
    {
        string path = Path.Combine(_root, directory);
        Directory.CreateDirectory(path);
        File.WriteAllText(Path.Combine(path, fileName), "fake");
    }
}
