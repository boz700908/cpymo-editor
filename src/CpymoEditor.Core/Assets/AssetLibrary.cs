namespace CpymoEditor.Core.Assets;

public sealed record AssetLibrary(
    string ProjectDirectory,
    IReadOnlyList<AssetItem> Assets);
