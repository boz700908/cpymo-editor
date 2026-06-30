namespace CpymoEditor.Pages;

public partial class AddEventPage : ContentPage
{
    public AddEventPage()
    {
        InitializeComponent();
        BindingContext = new ViewModels.AddEventViewModel();
    }
}
