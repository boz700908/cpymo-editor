using System.Windows.Input;

namespace CpymoEditor.ViewModels;

public sealed class AddEventTemplateViewModel
{
    public AddEventTemplateViewModel(string name, string description, Action<AddEventTemplateViewModel> select)
    {
        Name = name;
        Description = description;
        AccessibleName = name + "：" + description;
        SelectCommand = new RelayCommand(() => select(this));
    }

    public string Name { get; }

    public string Description { get; }

    public string AccessibleName { get; }

    public ICommand SelectCommand { get; }
}
