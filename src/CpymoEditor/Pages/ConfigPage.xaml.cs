namespace CpymoEditor.Pages;

public partial class ConfigPage : ContentPage
{
    public ConfigPage()
    {
        InitializeComponent();
        BindingContext = new ViewModels.ConfigViewModel();
    }
}
