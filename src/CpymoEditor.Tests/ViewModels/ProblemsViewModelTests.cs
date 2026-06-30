using CpymoEditor.Core.Tools;
using CpymoEditor.ViewModels;

namespace CpymoEditor.Tests.ViewModels;

public sealed class ProblemsViewModelTests
{
    [Fact]
    public void FromToolProblems_CreatesAccessibleProblemRows()
    {
        ToolProblem[] problems =
        [
            new(ToolProblemSeverity.Error, "Missing background asset.", "script/start.txt", 3),
            new(ToolProblemSeverity.Warning, "Missing startscript entry.", "gameconfig.txt")
        ];

        var viewModel = ProblemsViewModel.FromToolProblems(problems);

        Assert.Equal(2, viewModel.Problems.Count);
        Assert.Contains(viewModel.Problems, problem => problem.Severity == "错误");
        Assert.All(viewModel.Problems, problem => Assert.False(string.IsNullOrWhiteSpace(problem.AccessibleName)));
    }
}
