using CpymoEditor.Core.Events;

namespace CpymoEditor.Tests.Events;

public sealed class EventDocumentEditorTests
{
    [Fact]
    public void Insert_AddsEventAtRequestedIndex()
    {
        EventDocument document = BuildDocument("#raw 1", "#raw 3");
        ScriptEvent inserted = ScriptEvent.Raw(0, "#raw 2");

        EventDocument updated = EventDocumentEditor.Insert(document, index: 1, inserted);

        Assert.Equal(new[] { "#raw 1", "#raw 2", "#raw 3" }, updated.Events.Select(item => item.RawText));
    }

    [Fact]
    public void Delete_RemovesEventAtRequestedIndex()
    {
        EventDocument document = BuildDocument("#raw 1", "#raw 2", "#raw 3");

        EventDocument updated = EventDocumentEditor.Delete(document, index: 1);

        Assert.Equal(new[] { "#raw 1", "#raw 3" }, updated.Events.Select(item => item.RawText));
    }

    [Fact]
    public void Move_ReordersEvent()
    {
        EventDocument document = BuildDocument("#raw 1", "#raw 2", "#raw 3");

        EventDocument updated = EventDocumentEditor.Move(document, fromIndex: 2, toIndex: 0);

        Assert.Equal(new[] { "#raw 3", "#raw 1", "#raw 2" }, updated.Events.Select(item => item.RawText));
    }

    [Fact]
    public void Replace_UpdatesEventAtRequestedIndex()
    {
        EventDocument document = BuildDocument("#raw 1", "#raw old");

        EventDocument updated = EventDocumentEditor.Replace(document, index: 1, ScriptEvent.Raw(0, "#raw new"));

        Assert.Equal(new[] { "#raw 1", "#raw new" }, updated.Events.Select(item => item.RawText));
    }

    [Fact]
    public void Operations_RejectOutOfRangeIndex()
    {
        EventDocument document = BuildDocument("#raw 1");

        Assert.Throws<ArgumentOutOfRangeException>(() => EventDocumentEditor.Delete(document, index: 5));
        Assert.Throws<ArgumentOutOfRangeException>(() => EventDocumentEditor.Insert(document, index: 3, ScriptEvent.Raw(0, "#raw 2")));
        Assert.Throws<ArgumentOutOfRangeException>(() => EventDocumentEditor.Move(document, fromIndex: 0, toIndex: 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => EventDocumentEditor.Replace(document, index: -1, ScriptEvent.Raw(0, "#raw x")));
    }

    private static EventDocument BuildDocument(params string[] rawEvents)
    {
        ScriptEvent[] events = rawEvents
            .Select((raw, index) => ScriptEvent.Raw(index + 1, raw))
            .ToArray();

        return new EventDocument("script/start.txt", events);
    }
}
