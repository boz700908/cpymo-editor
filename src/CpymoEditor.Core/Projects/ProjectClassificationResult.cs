namespace CpymoEditor.Core.Projects;

public sealed record ProjectClassificationResult(
    ProjectKind Kind,
    bool CanOpen,
    IReadOnlyList<string> Reasons);
