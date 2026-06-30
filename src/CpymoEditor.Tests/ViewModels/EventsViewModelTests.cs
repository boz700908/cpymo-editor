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

    [Fact]
    public void DuplicateCommand_DuplicatesSelectedEventAfterExplicitAction()
    {
        var viewModel = new EventsViewModel(pageSize: 20);
        int initialCount = viewModel.Events.Count;

        viewModel.SelectEventCommand.Execute(viewModel.Events[0]);
        viewModel.DuplicateSelectedCommand.Execute(null);

        Assert.Equal(initialCount + 1, viewModel.Events.Count);
        Assert.Equal(viewModel.Events[0].Summary, viewModel.Events[1].Summary);
        Assert.False(string.IsNullOrWhiteSpace(viewModel.Events[1].AccessibleName));
    }

    [Fact]
    public void DeleteCommand_RemovesSelectedEventAfterExplicitAction()
    {
        var viewModel = new EventsViewModel(pageSize: 20);
        string secondSummary = viewModel.Events[1].Summary;

        viewModel.SelectEventCommand.Execute(viewModel.Events[0]);
        viewModel.DeleteSelectedCommand.Execute(null);

        Assert.Equal(secondSummary, viewModel.Events[0].Summary);
    }

    [Fact]
    public void MoveCommands_ReorderSelectedEventAfterExplicitAction()
    {
        var viewModel = new EventsViewModel(pageSize: 20);
        string firstSummary = viewModel.Events[0].Summary;

        viewModel.SelectEventCommand.Execute(viewModel.Events[0]);
        viewModel.MoveSelectedDownCommand.Execute(null);
        Assert.Equal(firstSummary, viewModel.Events[1].Summary);

        viewModel.SelectEventCommand.Execute(viewModel.Events[1]);
        viewModel.MoveSelectedUpCommand.Execute(null);
        Assert.Equal(firstSummary, viewModel.Events[0].Summary);
    }
}
