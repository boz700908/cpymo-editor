namespace CpymoEditor.Core.Events;

public sealed record ScriptEvent(
    ScriptEventKind Kind,
    int Line,
    string RawText,
    IReadOnlyDictionary<string, string> Parameters,
    IReadOnlyList<ScriptEvent> Children)
{
    public static ScriptEvent Raw(int line, string rawText)
    {
        return new ScriptEvent(ScriptEventKind.Raw, line, rawText, new Dictionary<string, string>(), Array.Empty<ScriptEvent>());
    }
}
