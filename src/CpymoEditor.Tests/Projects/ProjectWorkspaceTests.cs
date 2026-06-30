using CpymoEditor.Core.Projects;

namespace CpymoEditor.Tests.Projects;

public sealed class ProjectWorkspaceTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "cpymo-editor-workspace-" + Guid.NewGuid().ToString("N"));

    public ProjectWorkspaceTests()
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
    public void Open_LoadsNativePymoProjectIntoWorkspace()
    {
        Directory.CreateDirectory(Path.Combine(_root, "script"));
        Directory.CreateDirectory(Path.Combine(_root, "bg"));
        File.WriteAllText(Path.Combine(_root, "gameconfig.txt"), "scripttype,pymo\nstartscript,start\n");
        File.WriteAllText(Path.Combine(_root, "script", "start.txt"), "#say 智也,你好\n");
        File.WriteAllText(Path.Combine(_root, "bg", "BG001_H.png"), "");

        ProjectWorkspace workspace = ProjectWorkspace.Open(_root);

        Assert.True(workspace.CanEdit);
        Assert.Equal(ProjectKind.NativePymo, workspace.Kind);
        Assert.NotNull(workspace.Events);
        Assert.Contains(workspace.Assets.Assets, asset => asset.RelativePath.EndsWith("BG001_H.png", StringComparison.Ordinal));
        Assert.Equal("start", workspace.Config?.GetValue("startscript"));
    }

    [Fact]
    public void Open_RefusesYkmCompiledProductWithoutLoadingEditableContent()
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
            #say 智也,你好
            """);

        ProjectWorkspace workspace = ProjectWorkspace.Open(_root);

        Assert.False(workspace.CanEdit);
        Assert.Equal(ProjectKind.RejectedYkmCompiledProduct, workspace.Kind);
        Assert.Null(workspace.Events);
        Assert.NotEmpty(workspace.Messages);
    }
}
