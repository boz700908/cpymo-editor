namespace CpymoEditor.ViewModels;

public sealed class EventRowViewModel
{
    public EventRowViewModel(int index, string kind, string summary, string accessibleName, Action<EventRowViewModel> select)
    {
        Index = index;
        Kind = kind;
        Summary = summary;
        AccessibleName = accessibleName;
        SelectCommand = new RelayCommand(() => select(this));
    }

    public int Index { get; }

    public string Kind { get; }

    public string Summary { get; }

    public string AccessibleName { get; }

    public System.Windows.Input.ICommand SelectCommand { get; }
}
