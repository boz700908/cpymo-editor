using System.Collections.ObjectModel;
using System.Windows.Input;
using CpymoEditor.Core.Events;

namespace CpymoEditor.ViewModels;

public sealed class EventsViewModel
{
    private EventDocument _document;
    private readonly int _pageSize;
    private EventRowViewModel? _selectedEvent;

    public EventsViewModel()
        : this(20)
    {
    }

    public EventsViewModel(int pageSize)
    {
        _pageSize = pageSize;
        _document = PymoScriptParser.Parse(
            "script/start.txt",
            """
            #say 旁白,欢迎使用 CPyMO Editor
            #bg BG001_H,BG_FADE,500
            #bgm BGM00,1
            #sel 2
            开始
            继续
            #wait 1000
            """);

        PreviousPageCommand = new RelayCommand(PreviousPage, () => PageNumber > 1);
        NextPageCommand = new RelayCommand(NextPage, () => PageNumber < TotalPages);
        SelectEventCommand = new RelayCommand<EventRowViewModel>(SelectEvent);
        DuplicateSelectedCommand = new RelayCommand(DuplicateSelected, () => _selectedEvent is not null);
        LoadPage(1);
    }

    public ObservableCollection<EventRowViewModel> Events { get; } = [];

    public int PageNumber { get; private set; }

    public int TotalPages { get; private set; }

    public ICommand PreviousPageCommand { get; }

    public ICommand NextPageCommand { get; }

    public ICommand SelectEventCommand { get; }

    public ICommand DuplicateSelectedCommand { get; }

    private void PreviousPage()
    {
        LoadPage(Math.Max(1, PageNumber - 1));
    }

    private void NextPage()
    {
        LoadPage(Math.Min(TotalPages, PageNumber + 1));
    }

    private void SelectEvent(EventRowViewModel? item)
    {
        _selectedEvent = item;
        if (DuplicateSelectedCommand is RelayCommand duplicate)
        {
            duplicate.RaiseCanExecuteChanged();
        }
    }

    private void DuplicateSelected()
    {
        if (_selectedEvent is null)
        {
            return;
        }

        ScriptEvent item = _document.Events[_selectedEvent.Index];
        _document = EventDocumentEditor.Insert(_document, _selectedEvent.Index + 1, item);
        LoadPage(PageNumber);
    }

    private void LoadPage(int pageNumber)
    {
        EventPage page = EventPaginator.Paginate(_document, pageNumber, _pageSize);
        PageNumber = page.PageNumber;
        TotalPages = page.TotalPages;
        Events.Clear();

        int firstIndex = (page.PageNumber - 1) * page.PageSize;
        for (int index = 0; index < page.Events.Count; index++)
        {
            Events.Add(ToRow(page.Events[index], firstIndex + index));
        }

        if (PreviousPageCommand is RelayCommand previous)
        {
            previous.RaiseCanExecuteChanged();
        }

        if (NextPageCommand is RelayCommand next)
        {
            next.RaiseCanExecuteChanged();
        }

        if (DuplicateSelectedCommand is RelayCommand duplicate)
        {
            duplicate.RaiseCanExecuteChanged();
        }
    }

    private EventRowViewModel ToRow(ScriptEvent item, int index)
    {
        string kind = item.Kind switch
        {
            ScriptEventKind.Dialogue => "对话",
            ScriptEventKind.Background => "背景",
            ScriptEventKind.Selection => "选择",
            _ => "原始"
        };

        string summary = item.Kind switch
        {
            ScriptEventKind.Dialogue => item.Parameters.TryGetValue("speaker", out string? speaker) && !string.IsNullOrWhiteSpace(speaker)
                ? speaker + "：" + item.Parameters["text"]
                : item.Parameters.GetValueOrDefault("text", item.RawText),
            ScriptEventKind.Background => "背景：" + item.Parameters.GetValueOrDefault("asset", item.RawText),
            ScriptEventKind.Selection => "选择：" + item.Children.Count + " 项",
            _ => item.RawText
        };

        return new EventRowViewModel(index, kind, summary, kind + "：" + summary, SelectEvent);
    }
}
