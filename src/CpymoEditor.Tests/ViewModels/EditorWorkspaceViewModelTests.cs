using CpymoEditor.ViewModels;

namespace CpymoEditor.Tests.ViewModels;

public sealed class EditorWorkspaceViewModelTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "cpymo-editor-vm-workspace-" + Guid.NewGuid().ToString("N"));

    public EditorWorkspaceViewModelTests()
    {
        Directory.CreateDirectory(_root);
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }

    [Fact]
    public void OpenProject_LoadsEditableNativePymoIntoChildViewModels()
    {
        Directory.CreateDirectory(Path.Combine(_root, "script"));
        File.WriteAllText(Path.Combine(_root, "gameconfig.txt"), "scripttype,pymo\nstartscript,start\n");
        File.WriteAllText(Path.Combine(_root, "script", "start.txt"), "#say 智也,你好\n");

        var viewModel = new EditorWorkspaceViewModel();

        viewModel.OpenProject(_root);

        Assert.True(viewModel.CanEdit);
        Assert.NotEmpty(viewModel.Events.Events);
        Assert.Contains("#say 智也,你好", viewModel.Source.SourceText);
        Assert.Empty(viewModel.Problems.Problems);
    }

    [Fact]
    public void OpenProject_ReportsRefusalForYkmCompiledProduct()
    {
        Directory.CreateDirectory(Path.Combine(_root, "script"));
        File.WriteAllText(Path.Combine(_root, "gameconfig.txt"), "scripttype,pymo\nstartscript,start\n");
        File.WriteAllText(
            Path.Combine(_root, "script", "start.txt"),
            """
            ;YKMDBG;PD:\game\startup.ykm
            ;YKMDBG;Pstartup
            ;YKMDBG;Pload
            ;YKMDBG;L10;F0;S1;E
            """);

        var viewModel = new EditorWorkspaceViewModel();

        viewModel.OpenProject(_root);

        Assert.False(viewModel.CanEdit);
        Assert.NotEmpty(viewModel.Problems.Problems);
        Assert.Contains("YukimiScript", viewModel.StatusMessage);
    }
}
