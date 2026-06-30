using CpymoEditor.Core.Events;

namespace CpymoEditor.Tests.Events;

public sealed class ScriptEventFactoryTests
{
    [Fact]
    public void Dialogue_CreatesSayEventWithSpeaker()
    {
        ScriptEvent item = ScriptEventFactory.Dialogue("智也", "你好");

        Assert.Equal(ScriptEventKind.Dialogue, item.Kind);
        Assert.Equal("#say 智也,你好", item.RawText);
        Assert.Equal("智也", item.Parameters["speaker"]);
        Assert.Equal("你好", item.Parameters["text"]);
    }

    [Fact]
    public void Background_CreatesBgEventWithOptionalTransitionAndTime()
    {
        ScriptEvent item = ScriptEventFactory.Background("BG001_H", "BG_FADE", 500);

        Assert.Equal(ScriptEventKind.Background, item.Kind);
        Assert.Equal("#bg BG001_H,BG_FADE,500", item.RawText);
    }

    [Fact]
    public void Bgm_CreatesRawBgmCommand()
    {
        ScriptEvent item = ScriptEventFactory.Bgm("BGM00", loop: true);

        Assert.Equal(ScriptEventKind.Raw, item.Kind);
        Assert.Equal("#bgm BGM00,1", item.RawText);
    }

    [Fact]
    public void Wait_CreatesRawWaitCommand()
    {
        ScriptEvent item = ScriptEventFactory.Wait(2000);

        Assert.Equal("#wait 2000", item.RawText);
    }
}
