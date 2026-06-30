using System.Collections.ObjectModel;
using CpymoEditor.Core.Compilation;
using CpymoEditor.Core.Tools;

namespace CpymoEditor.ViewModels;

public sealed class ProblemsViewModel
{
    public ProblemsViewModel()
    {
    }

    private ProblemsViewModel(IEnumerable<ProblemRowViewModel> problems)
    {
        foreach (ProblemRowViewModel problem in problems)
        {
            Problems.Add(problem);
        }
    }

    public ObservableCollection<ProblemRowViewModel> Problems { get; } = [];

    public static ProblemsViewModel FromToolProblems(IEnumerable<ToolProblem> problems)
    {
        return new ProblemsViewModel(problems.Select(problem => ToRow(
            problem.Severity,
            problem.Message,
            problem.File,
            problem.Line)));
    }

    public static ProblemsViewModel FromCompilerDiagnostics(IEnumerable<CompilerDiagnostic> diagnostics)
    {
        return new ProblemsViewModel(diagnostics.Select(diagnostic => ToRow(
            diagnostic.Severity,
            diagnostic.Message,
            diagnostic.File,
            diagnostic.Line)));
    }

    private static ProblemRowViewModel ToRow(Enum severity, string message, string? file, int? line)
    {
        string severityText = severity.ToString() switch
        {
            nameof(ToolProblemSeverity.Error) => "错误",
            nameof(ToolProblemSeverity.Warning) => "警告",
            _ => "信息"
        };

        string location = string.IsNullOrWhiteSpace(file)
            ? string.Empty
            : line.HasValue
                ? file + ":" + line.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)
                : file;

        string accessibleName = string.IsNullOrWhiteSpace(location)
            ? severityText + "：" + message
            : severityText + "：" + message + "，位置：" + location;

        return new ProblemRowViewModel(severityText, message, location, accessibleName);
    }
}
