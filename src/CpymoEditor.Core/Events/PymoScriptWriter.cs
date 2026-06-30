using System.Text;

namespace CpymoEditor.Core.Events;

public static class PymoScriptWriter
{
    public static string Write(EventDocument document)
    {
        var builder = new StringBuilder();

        foreach (ScriptEvent item in document.Events)
        {
            WriteEvent(builder, item);
        }

        return builder.ToString();
    }

    private static void WriteEvent(StringBuilder builder, ScriptEvent item)
    {
        switch (item.Kind)
        {
            case ScriptEventKind.Dialogue:
            case ScriptEventKind.Background:
            case ScriptEventKind.Raw:
                AppendLine(builder, item.RawText);
                break;
            case ScriptEventKind.Selection:
                AppendLine(builder, item.RawText);
                foreach (ScriptEvent child in item.Children)
                {
                    AppendLine(builder, child.RawText);
                }
                break;
        }
    }

    private static void AppendLine(StringBuilder builder, string line)
    {
        builder.Append(line);
        builder.Append('\n');
    }
}
