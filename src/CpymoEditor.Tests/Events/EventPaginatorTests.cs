using CpymoEditor.Core.Events;

namespace CpymoEditor.Tests.Events;

public sealed class EventPaginatorTests
{
    [Fact]
    public void Paginate_ReturnsRequestedPageWithTotalPageCount()
    {
        EventDocument document = BuildDocument(25);

        EventPage page = EventPaginator.Paginate(document, pageNumber: 2, pageSize: 10);

        Assert.Equal(2, page.PageNumber);
        Assert.Equal(10, page.PageSize);
        Assert.Equal(3, page.TotalPages);
        Assert.Equal(25, page.TotalEvents);
        Assert.Equal(10, page.Events.Count);
        Assert.Equal(11, page.Events[0].Line);
        Assert.True(page.HasPreviousPage);
        Assert.True(page.HasNextPage);
    }

    [Fact]
    public void Paginate_ClampsPageNumberToValidRange()
    {
        EventDocument document = BuildDocument(5);

        EventPage page = EventPaginator.Paginate(document, pageNumber: 99, pageSize: 10);

        Assert.Equal(1, page.PageNumber);
        Assert.Equal(1, page.TotalPages);
        Assert.Equal(5, page.Events.Count);
        Assert.False(page.HasPreviousPage);
        Assert.False(page.HasNextPage);
    }

    [Fact]
    public void Paginate_RejectsInvalidPageSize()
    {
        EventDocument document = BuildDocument(1);

        Assert.Throws<ArgumentOutOfRangeException>(() => EventPaginator.Paginate(document, pageNumber: 1, pageSize: 0));
    }

    private static EventDocument BuildDocument(int count)
    {
        var events = Enumerable.Range(1, count)
            .Select(line => ScriptEvent.Raw(line, "#raw " + line))
            .ToArray();

        return new EventDocument("script/start.txt", events);
    }
}
