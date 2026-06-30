using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace CpymoEditor.Core.Compilation;

public sealed class YukimiScriptPymoCompilerService : IYkmCompilerService
{
    public Task<CompileResult> CompileToPymoAsync(YkmCompileRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Directory.CreateDirectory(request.OutputDirectory);

        string scriptName = Path.GetFileNameWithoutExtension(request.SourcePath);
        string outputFile = Path.Combine(request.OutputDirectory, scriptName + ".txt");

        try
        {
            FSharpList<string> libs = ListModule.OfSeq(request.LibraryPaths);
            FSharpResult<YukimiScript.Parser.Dom, Exception> loadedLibs = YukimiScript.Parser.CompilePipe.loadLibs(libs);
            if (loadedLibs.IsError)
            {
                return Task.FromResult(Failed(loadedLibs.ErrorValue.Message));
            }

            FSharpResult<YukimiScript.Parser.Intermediate, Exception> compiled =
                YukimiScript.Parser.CompilePipe.compile(loadedLibs.ResultValue, request.SourcePath);
            if (compiled.IsError)
            {
                return Task.FromResult(Failed(compiled.ErrorValue.Message));
            }

            FSharpResult<string, Unit> generated =
                YukimiScript.CodeGen.PyMO.generateScript(false, compiled.ResultValue, scriptName);
            if (generated.IsError)
            {
                return Task.FromResult(Failed("PyMO code generation failed."));
            }

            File.WriteAllText(outputFile, generated.ResultValue, System.Text.Encoding.UTF8);

            return Task.FromResult(new CompileResult(
                Succeeded: true,
                GeneratedFiles: new[] { outputFile },
                Diagnostics: Array.Empty<CompilerDiagnostic>(),
                LogLines: new[] { $"Compiled {request.SourcePath} to {outputFile}." }));
        }
        catch (Exception exception)
        {
            return Task.FromResult(Failed(exception.Message));
        }
    }

    private static CompileResult Failed(string message)
    {
        return new CompileResult(
            Succeeded: false,
            GeneratedFiles: Array.Empty<string>(),
            Diagnostics: new[] { new CompilerDiagnostic(CompilerDiagnosticSeverity.Error, null, null, null, message) },
            LogLines: Array.Empty<string>());
    }
}
