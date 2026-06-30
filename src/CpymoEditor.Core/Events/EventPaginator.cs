namespace CpymoEditor.Core.Events;

public static class EventPaginator
{
    public static EventPage Paginate(EventDocument document, int pageNumber, int pageSize)
    {
        if (pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");
        }

        int totalEvents = document.Events.Count;
        int totalPages = Math.Max(1, (int)Math.Ceiling(totalEvents / (double)pageSize));
        int clampedPageNumber = Math.Clamp(pageNumber, 1, totalPages);
        ScriptEvent[] events = document.Events
            .Skip((clampedPageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        return new EventPage(clampedPageNumber, pageSize, totalPages, totalEvents, events);
    }
}
