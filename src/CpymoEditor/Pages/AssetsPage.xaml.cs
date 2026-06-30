namespace CpymoEditor.Pages;

public partial class AssetsPage : ContentPage
{
    public AssetsPage()
    {
        InitializeComponent();
        BindingContext = new ViewModels.AssetsViewModel();
    }
}
