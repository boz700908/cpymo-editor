using CpymoEditor.Core.Configuration;

namespace CpymoEditor.Tests.Configuration;

public sealed class GameConfigDocumentTests
{
    [Fact]
    public void Parse_ReadsCommaSeparatedEntriesWithoutTrimmingValues()
    {
        GameConfigDocument document = GameConfigDocument.Parse(
            """
            gametitle,秋之回忆\n作者
            startscript,start
            textcolor,#ffffff
            """);

        Assert.Equal("秋之回忆\\n作者", document.GetValue("gametitle"));
        Assert.Equal("start", document.GetValue("startscript"));
        Assert.Equal("#ffffff", document.GetValue("textcolor"));
    }

    [Fact]
    public void SetValue_UpdatesExistingEntryAndPreservesOrder()
    {
        GameConfigDocument document = GameConfigDocument.Parse("scripttype,pymo\nstartscript,start\n");

        document.SetValue("startscript", "opening");

        Assert.Equal("scripttype,pymo\nstartscript,opening\n", document.Write());
    }

    [Fact]
    public void SetValue_AppendsNewEntry()
    {
        GameConfigDocument document = GameConfigDocument.Parse("scripttype,pymo\n");

        document.SetValue("platform", "pygame");

        Assert.Equal("scripttype,pymo\nplatform,pygame\n", document.Write());
    }

    [Fact]
    public void Parse_RejectsEntryWithSpacesAroundComma()
    {
        Assert.Throws<FormatException>(() => GameConfigDocument.Parse("startscript, start\n"));
    }
}
