using CpymoEditor.ViewModels;

namespace CpymoEditor.Tests.ViewModels;

public sealed class EventsViewModelTests
{
    [Fact]
    public void Constructor_LoadsSampleDocumentAndFirstPage()
    {
        var viewModel = new EventsViewModel();

        Assert.NotEmpty(viewModel.Events);
        Assert.Equal(1, viewModel.PageNumber);
        Assert.True(viewModel.TotalPages >= 1);
    }

    [Fact]
    public void NextPage_MovesForwardWhenAvailable()
    {
        var viewModel = new EventsViewModel(pageSize: 1);

        viewModel.NextPageCommand.Execute(null);

        Assert.Equal(2, viewModel.PageNumber);
    }

    [Fact]
    public void PreviousPage_DoesNotMoveBeforeFirstPage()
    {
        var viewModel = new EventsViewModel(pageSize: 1);

        viewModel.PreviousPageCommand.Execute(null);

        Assert.Equal(1, viewModel.PageNumber);
    }
}
