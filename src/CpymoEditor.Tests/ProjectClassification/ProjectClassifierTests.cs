using CpymoEditor.Core.Projects;

namespace CpymoEditor.Tests.ProjectClassification;

public sealed class ProjectClassifierTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "cpymo-editor-tests", Guid.NewGuid().ToString("N"));

    public ProjectClassifierTests()
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
    public void Classify_ReturnsYkmSource_WhenDirectoryContainsYkmFile()
    {
        File.WriteAllText(Path.Combine(_root, "main.ykm"), "- scene \"main\"\n旁白:你好\n");

        ProjectClassificationResult result = ProjectClassifier.Classify(_root);

        Assert.Equal(ProjectKind.YkmSource, result.Kind);
        Assert.True(result.CanOpen);
        Assert.Empty(result.Reasons);
    }

    [Fact]
    public void Classify_ReturnsNativePymo_WhenDirectoryContainsGameconfigAndScriptTxt()
    {
        Directory.CreateDirectory(Path.Combine(_root, "script"));
        File.WriteAllText(Path.Combine(_root, "gameconfig.txt"), "scripttype,pymo\nstartscript,start\n");
        File.WriteAllText(Path.Combine(_root, "script", "start.txt"), "#say 你好\n");

        ProjectClassificationResult result = ProjectClassifier.Classify(_root);

        Assert.Equal(ProjectKind.NativePymo, result.Kind);
        Assert.True(result.CanOpen);
        Assert.Empty(result.Reasons);
    }

    [Fact]
    public void Classify_RejectsYkmCompiledProduct_WhenScriptContainsYkmDebugMetadata()
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
            #label SCN_startup
            #say 角色,内容
            """);

        ProjectClassificationResult result = ProjectClassifier.Classify(_root);

        Assert.Equal(ProjectKind.RejectedYkmCompiledProduct, result.Kind);
        Assert.False(result.CanOpen);
        Assert.Contains(result.Reasons, reason => reason.Contains("YukimiScript", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Classify_ReturnsUnknown_WhenNoSupportedProjectFilesExist()
    {
        ProjectClassificationResult result = ProjectClassifier.Classify(_root);

        Assert.Equal(ProjectKind.Unknown, result.Kind);
        Assert.False(result.CanOpen);
        Assert.NotEmpty(result.Reasons);
    }
}
