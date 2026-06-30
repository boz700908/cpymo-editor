namespace CpymoEditor.Core.Events;

public static class ScriptEventFactory
{
    public static ScriptEvent Dialogue(string speaker, string text)
    {
        string raw = string.IsNullOrWhiteSpace(speaker)
            ? "#say " + text
            : "#say " + speaker + "," + text;

        return new ScriptEvent(
            ScriptEventKind.Dialogue,
            0,
            raw,
            new Dictionary<string, string>
            {
                ["speaker"] = speaker,
                ["text"] = text
            },
            Array.Empty<ScriptEvent>());
    }

    public static ScriptEvent Background(string asset, string? transition = null, int? time = null)
    {
        var parts = new List<string> { asset };
        if (!string.IsNullOrWhiteSpace(transition))
        {
            parts.Add(transition);
        }

        if (time.HasValue)
        {
            parts.Add(time.Value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        string raw = "#bg " + string.Join(",", parts);
        return new ScriptEvent(
            ScriptEventKind.Background,
            0,
            raw,
            new Dictionary<string, string>
            {
                ["asset"] = asset,
                ["transition"] = transition ?? "",
                ["time"] = time?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? ""
            },
            Array.Empty<ScriptEvent>());
    }

    public static ScriptEvent Bgm(string asset, bool loop)
    {
        return ScriptEvent.Raw(0, "#bgm " + asset + "," + (loop ? "1" : "0"));
    }

    public static ScriptEvent Wait(int milliseconds)
    {
        return ScriptEvent.Raw(0, "#wait " + milliseconds.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }
}
