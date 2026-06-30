namespace CpymoEditor.ViewModels;

public sealed record ToolCommandViewModel(
    string Name,
    string Description,
    string Platforms,
    string Status,
    string AccessibleName);
