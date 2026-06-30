namespace CpymoEditor.Pages;

public partial class SourcePage : ContentPage
{
    public SourcePage()
    {
        InitializeComponent();
        BindingContext = new ViewModels.SourceViewModel();
    }
}
