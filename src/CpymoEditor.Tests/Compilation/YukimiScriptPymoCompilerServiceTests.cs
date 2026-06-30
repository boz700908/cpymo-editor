using CpymoEditor.Core.Compilation;

namespace CpymoEditor.Tests.Compilation;

public sealed class YukimiScriptPymoCompilerServiceTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "cpymo-editor-ykm-tests", Guid.NewGuid().ToString("N"));

    public YukimiScriptPymoCompilerServiceTests()
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
    public async Task CompileToPymoAsync_CompilesYkmSourceWithLibpymo()
    {
        string sourcePath = Path.Combine(_root, "main.ykm");
        string outputDirectory = Path.Combine(_root, "script");
        string libpymoPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "external", "CPyMO", "libpymo.ykm"));

        await File.WriteAllTextAsync(sourcePath, "- scene \"main\"\n旁白:你好\n");

        var service = new YukimiScriptPymoCompilerService();
        CompileResult result = await service.CompileToPymoAsync(
            new YkmCompileRequest(sourcePath, outputDirectory, new[] { libpymoPath }),
            CancellationToken.None);

        Assert.True(result.Succeeded, string.Join(Environment.NewLine, result.Diagnostics.Select(diagnostic => diagnostic.Message)));
        Assert.Single(result.GeneratedFiles);
        Assert.True(File.Exists(result.GeneratedFiles[0]));
        Assert.Contains("#say", await File.ReadAllTextAsync(result.GeneratedFiles[0]));
    }
}
