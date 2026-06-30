using System.Collections.ObjectModel;
using CpymoEditor.Core.Events;
using System.Windows.Input;

namespace CpymoEditor.ViewModels;

public sealed class EventsViewModel
{
    private readonly EventDocument _document;
    private readonly int _pageSize;

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
        LoadPage(1);
    }

    public ObservableCollection<EventRowViewModel> Events { get; } = [];

    public int PageNumber { get; private set; }

    public int TotalPages { get; private set; }

    public ICommand PreviousPageCommand { get; }

    public ICommand NextPageCommand { get; }

    private void PreviousPage()
    {
        LoadPage(Math.Max(1, PageNumber - 1));
    }

    private void NextPage()
    {
        LoadPage(Math.Min(TotalPages, PageNumber + 1));
    }

    private void LoadPage(int pageNumber)
    {
        EventPage page = EventPaginator.Paginate(_document, pageNumber, _pageSize);
        PageNumber = page.PageNumber;
        TotalPages = page.TotalPages;
        Events.Clear();

        foreach (ScriptEvent item in page.Events)
        {
            Events.Add(ToRow(item));
        }

        if (PreviousPageCommand is RelayCommand previous)
        {
            previous.RaiseCanExecuteChanged();
        }

        if (NextPageCommand is RelayCommand next)
        {
            next.RaiseCanExecuteChanged();
        }
    }

    private static EventRowViewModel ToRow(ScriptEvent item)
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

        return new EventRowViewModel(kind, summary, kind + "，" + summary);
    }
}
