namespace CpymoEditor.Core.Compilation;

public interface IYkmCompilerService
{
    Task<CompileResult> CompileToPymoAsync(YkmCompileRequest request, CancellationToken cancellationToken);
}
