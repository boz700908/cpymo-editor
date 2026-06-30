namespace CpymoEditor.Core.Events;

public static class PymoScriptParser
{
    public static EventDocument Parse(string path, string source)
    {
        string[] lines = source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        var events = new List<ScriptEvent>();

        for (int index = 0; index < lines.Length; index++)
        {
            string line = lines[index];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            int lineNumber = index + 1;
            string trimmed = line.Trim();

            if (trimmed.StartsWith("#say ", StringComparison.Ordinal))
            {
                events.Add(ParseSay(lineNumber, trimmed));
            }
            else if (trimmed.StartsWith("#bg ", StringComparison.Ordinal))
            {
                events.Add(ParseBackground(lineNumber, trimmed));
            }
            else if (trimmed.StartsWith("#sel ", StringComparison.Ordinal))
            {
                ScriptEvent selection = ParseSelection(lineNumber, trimmed, lines, ref index);
                events.Add(selection);
            }
            else if (trimmed.StartsWith('#'))
            {
                events.Add(ScriptEvent.Raw(lineNumber, trimmed));
            }
        }

        return new EventDocument(path, events);
    }

    private static ScriptEvent ParseSay(int lineNumber, string line)
    {
        string content = line["#say ".Length..];
        string speaker = "";
        string text = content;

        int comma = content.IndexOf(',');
        if (comma >= 0)
        {
            speaker = content[..comma];
            text = content[(comma + 1)..];
        }

        return new ScriptEvent(
            ScriptEventKind.Dialogue,
            lineNumber,
            line,
            new Dictionary<string, string>
            {
                ["speaker"] = speaker,
                ["text"] = text
            },
            Array.Empty<ScriptEvent>());
    }

    private static ScriptEvent ParseBackground(int lineNumber, string line)
    {
        string[] arguments = line["#bg ".Length..].Split(',', StringSplitOptions.TrimEntries);
        var parameters = new Dictionary<string, string>
        {
            ["asset"] = arguments.ElementAtOrDefault(0) ?? ""
        };

        if (arguments.Length > 1)
        {
            parameters["transition"] = arguments[1];
        }

        if (arguments.Length > 2)
        {
            parameters["time"] = arguments[2];
        }

        return new ScriptEvent(ScriptEventKind.Background, lineNumber, line, parameters, Array.Empty<ScriptEvent>());
    }

    private static ScriptEvent ParseSelection(int lineNumber, string line, string[] lines, ref int index)
    {
        string[] arguments = line["#sel ".Length..].Split(',', StringSplitOptions.TrimEntries);
        string countText = arguments.ElementAtOrDefault(0) ?? "0";
        int.TryParse(countText, out int count);

        var choices = new List<ScriptEvent>();
        for (int choice = 0; choice < count && index + 1 < lines.Length; choice++)
        {
            index++;
            choices.Add(ScriptEvent.Raw(index + 1, lines[index].Trim()));
        }

        return new ScriptEvent(
            ScriptEventKind.Selection,
            lineNumber,
            line,
            new Dictionary<string, string> { ["count"] = countText },
            choices);
    }
}
