using CpymoEditor.ViewModels;

namespace CpymoEditor.Tests.ViewModels;

public sealed class AddEventViewModelTests
{
    [Fact]
    public void Constructor_LoadsAccessibleEventTemplatesByCategory()
    {
        var viewModel = new AddEventViewModel();

        Assert.NotEmpty(viewModel.Categories);
        Assert.Contains(viewModel.Categories, category => category.Name == "文本");
        Assert.Contains(viewModel.Categories, category => category.Name == "图像");
        Assert.Contains(viewModel.Categories, category => category.Name == "声音");
        Assert.Contains(viewModel.Categories.SelectMany(category => category.Events), item => item.Name == "对话");
        Assert.All(
            viewModel.Categories.SelectMany(category => category.Events),
            item => Assert.False(string.IsNullOrWhiteSpace(item.AccessibleName)));
    }
}
