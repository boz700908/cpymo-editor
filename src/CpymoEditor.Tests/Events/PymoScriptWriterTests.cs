using CpymoEditor.Core.Events;

namespace CpymoEditor.Tests.Events;

public sealed class PymoScriptWriterTests
{
    [Fact]
    public void Write_RoundTripsParsedKnownAndRawEvents()
    {
        EventDocument document = PymoScriptParser.Parse(
            "script/start.txt",
            """
            #say 智也,你好
            #bg BG001_H,BG_FADE,500
            #custom untouched
            """);

        string output = PymoScriptWriter.Write(document);

        Assert.Equal(
            """
            #say 智也,你好
            #bg BG001_H,BG_FADE,500
            #custom untouched
            """ + "\n",
            output);
    }

    [Fact]
    public void Write_WritesSelectionChoicesAsMultilineSel()
    {
        EventDocument document = PymoScriptParser.Parse(
            "script/start.txt",
            """
            #sel 2
            选项一
            选项二
            """);

        string output = PymoScriptWriter.Write(document);

        Assert.Equal(
            """
            #sel 2
            选项一
            选项二
            """ + "\n",
            output);
    }
}
