namespace CpymoEditor.ViewModels;

public sealed record ProblemRowViewModel(
    string Severity,
    string Message,
    string Location,
    string AccessibleName);
