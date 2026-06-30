namespace CpymoEditor.Core.Assets;

public sealed record AssetItem(
    AssetKind Kind,
    string Name,
    string Extension,
    string RelativePath,
    string FullPath);
