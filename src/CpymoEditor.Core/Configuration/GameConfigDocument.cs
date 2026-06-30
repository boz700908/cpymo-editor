namespace CpymoEditor.Core.Configuration;

public sealed class GameConfigDocument
{
    private readonly List<GameConfigEntry> _entries = [];

    private GameConfigDocument()
    {
    }

    public static GameConfigDocument Parse(string source)
    {
        var document = new GameConfigDocument();
        string[] lines = source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

        for (int index = 0; index < lines.Length; index++)
        {
            string line = lines[index];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            int comma = line.IndexOf(',');
            if (comma <= 0 || comma == line.Length - 1)
            {
                throw new FormatException($"Invalid gameconfig entry at line {index + 1}.");
            }

            string key = line[..comma];
            string value = line[(comma + 1)..];
            if (key.EndsWith(' ') || value.StartsWith(' '))
            {
                throw new FormatException($"Spaces around comma are not allowed at line {index + 1}.");
            }

            document._entries.Add(new GameConfigEntry(key, value));
        }

        return document;
    }

    public string? GetValue(string key)
    {
        return _entries.FirstOrDefault(entry => entry.Key == key)?.Value;
    }

    public void SetValue(string key, string value)
    {
        for (int index = 0; index < _entries.Count; index++)
        {
            if (_entries[index].Key == key)
            {
                _entries[index] = new GameConfigEntry(key, value);
                return;
            }
        }

        _entries.Add(new GameConfigEntry(key, value));
    }

    public string Write()
    {
        return string.Concat(_entries.Select(entry => entry.Key + "," + entry.Value + "\n"));
    }

    private sealed record GameConfigEntry(string Key, string Value);
}
