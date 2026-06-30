namespace CpymoEditor.Core.Events;

public static class EventDocumentEditor
{
    public static EventDocument Insert(EventDocument document, int index, ScriptEvent item)
    {
        if (index < 0 || index > document.Events.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        var events = document.Events.ToList();
        events.Insert(index, item);
        return document with { Events = events };
    }

    public static EventDocument Delete(EventDocument document, int index)
    {
        ValidateExistingIndex(document, index);

        var events = document.Events.ToList();
        events.RemoveAt(index);
        return document with { Events = events };
    }

    public static EventDocument Move(EventDocument document, int fromIndex, int toIndex)
    {
        ValidateExistingIndex(document, fromIndex);
        ValidateExistingIndex(document, toIndex);

        var events = document.Events.ToList();
        ScriptEvent item = events[fromIndex];
        events.RemoveAt(fromIndex);
        events.Insert(toIndex, item);
        return document with { Events = events };
    }

    public static EventDocument Replace(EventDocument document, int index, ScriptEvent item)
    {
        ValidateExistingIndex(document, index);

        var events = document.Events.ToList();
        events[index] = item;
        return document with { Events = events };
    }

    private static void ValidateExistingIndex(EventDocument document, int index)
    {
        if (index < 0 || index >= document.Events.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
    }
}
