namespace CpymoEditor.Pages;

public partial class ProblemsPage : ContentPage
{
    public ProblemsPage()
    {
        InitializeComponent();
        BindingContext = new ViewModels.ProblemsViewModel();
    }
}
