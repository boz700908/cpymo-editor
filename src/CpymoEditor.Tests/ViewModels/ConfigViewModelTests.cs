using CpymoEditor.Core.Configuration;
using CpymoEditor.ViewModels;

namespace CpymoEditor.Tests.ViewModels;

public sealed class ConfigViewModelTests
{
    [Fact]
    public void SaveCommand_WritesStrictCommaSeparatedGameConfig()
    {
        GameConfigDocument document = GameConfigDocument.Parse("scripttype,pymo\nstartscript,start\n");
        var viewModel = new ConfigViewModel(document)
        {
            GameTitle = "秋之回忆",
            StartScript = "opening",
            BackgroundFormat = ".png"
        };

        viewModel.SaveCommand.Execute(null);

        Assert.Equal(
            "scripttype,pymo\nstartscript,opening\ngametitle,秋之回忆\nbgformat,.png\n",
            viewModel.SerializedConfig);
        Assert.False(string.IsNullOrWhiteSpace(viewModel.SaveStatus));
    }
}
