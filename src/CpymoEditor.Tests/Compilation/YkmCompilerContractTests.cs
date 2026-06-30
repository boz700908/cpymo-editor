using CpymoEditor.Core.Compilation;

namespace CpymoEditor.Tests.Compilation;

public sealed class YkmCompilerContractTests
{
    [Fact]
    public void Diagnostic_CarriesSourceLocationAndMessage()
    {
        var diagnostic = new CompilerDiagnostic(
            CompilerDiagnosticSeverity.Error,
            "main.ykm",
            7,
            3,
            "Unknown command.");

        Assert.Equal(CompilerDiagnosticSeverity.Error, diagnostic.Severity);
        Assert.Equal("main.ykm", diagnostic.File);
        Assert.Equal(7, diagnostic.Line);
        Assert.Equal(3, diagnostic.Column);
        Assert.Equal("Unknown command.", diagnostic.Message);
    }

    [Fact]
    public void CompileResult_SuccessRequiresNoErrorDiagnostics()
    {
        var success = new CompileResult(
            Succeeded: true,
            GeneratedFiles: new[] { "script/main.txt" },
            Diagnostics: Array.Empty<CompilerDiagnostic>(),
            LogLines: new[] { "Compiled main.ykm." });

        Assert.True(success.Succeeded);
        Assert.Equal("script/main.txt", success.GeneratedFiles[0]);
        Assert.Empty(success.Diagnostics);
        Assert.Contains("Compiled", success.LogLines[0]);
    }
}
