namespace CpymoEditor.Core.Events;

public sealed record EventPage(
    int PageNumber,
    int PageSize,
    int TotalPages,
    int TotalEvents,
    IReadOnlyList<ScriptEvent> Events)
{
    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;
}
