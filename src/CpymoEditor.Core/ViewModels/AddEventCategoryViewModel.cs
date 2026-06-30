using System.Collections.ObjectModel;

namespace CpymoEditor.ViewModels;

public sealed class AddEventCategoryViewModel
{
    public AddEventCategoryViewModel(string name, IEnumerable<AddEventTemplateViewModel> events)
    {
        Name = name;
        Events = new ObservableCollection<AddEventTemplateViewModel>(events);
    }

    public string Name { get; }

    public ObservableCollection<AddEventTemplateViewModel> Events { get; }
}
