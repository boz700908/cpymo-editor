namespace CpymoEditor.ViewModels;

public sealed record AssetRowViewModel(
    string Kind,
    string Name,
    string RelativePath,
    string AccessibleName);
