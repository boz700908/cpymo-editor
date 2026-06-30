namespace CpymoEditor.Core.Events;

public sealed record EventDocument(
    string Path,
    IReadOnlyList<ScriptEvent> Events);
