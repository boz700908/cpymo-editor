using CpymoEditor.Core.Events;

namespace CpymoEditor.Tests.Events;

public sealed class PymoScriptParserTests
{
    [Fact]
    public void Parse_ReturnsDialogueEventForSayWithSpeaker()
    {
        EventDocument document = PymoScriptParser.Parse("script/start.txt", "#say 智也,你好\n");

        ScriptEvent item = Assert.Single(document.Events);
        Assert.Equal(ScriptEventKind.Dialogue, item.Kind);
        Assert.Equal(1, item.Line);
        Assert.Equal("智也", item.Parameters["speaker"]);
        Assert.Equal("你好", item.Parameters["text"]);
    }

    [Fact]
    public void Parse_ReturnsBackgroundEventForBgCommand()
    {
        EventDocument document = PymoScriptParser.Parse("script/start.txt", "#bg BG001_H,BG_FADE,500\n");

        ScriptEvent item = Assert.Single(document.Events);
        Assert.Equal(ScriptEventKind.Background, item.Kind);
        Assert.Equal("BG001_H", item.Parameters["asset"]);
        Assert.Equal("BG_FADE", item.Parameters["transition"]);
        Assert.Equal("500", item.Parameters["time"]);
    }

    [Fact]
    public void Parse_GroupsMultilineSelIntoSingleSelectionEvent()
    {
        EventDocument document = PymoScriptParser.Parse(
            "script/start.txt",
            """
            #sel 2
            去放焰火吧
            到我家里去坐坐吧？
            #say 结束
            """);

        Assert.Equal(2, document.Events.Count);
        ScriptEvent selection = document.Events[0];
        Assert.Equal(ScriptEventKind.Selection, selection.Kind);
        Assert.Equal("2", selection.Parameters["count"]);
        Assert.Equal(new[] { "去放焰火吧", "到我家里去坐坐吧？" }, selection.Children.Select(child => child.RawText));
    }

    [Fact]
    public void Parse_PreservesUnknownCommandAsRawEvent()
    {
        EventDocument document = PymoScriptParser.Parse("script/start.txt", "#custom something\n");

        ScriptEvent item = Assert.Single(document.Events);
        Assert.Equal(ScriptEventKind.Raw, item.Kind);
        Assert.Equal("#custom something", item.RawText);
    }
}
