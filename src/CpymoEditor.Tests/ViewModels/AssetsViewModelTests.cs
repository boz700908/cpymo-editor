using CpymoEditor.ViewModels;

namespace CpymoEditor.Tests.ViewModels;

public sealed class AssetsViewModelTests
{
    [Fact]
    public void Constructor_LoadsSampleCategorizedAssets()
    {
        var viewModel = new AssetsViewModel();

        Assert.NotEmpty(viewModel.Assets);
        Assert.Contains(viewModel.Assets, asset => asset.Kind == "背景");
        Assert.All(viewModel.Assets, asset => Assert.False(string.IsNullOrWhiteSpace(asset.AccessibleName)));
    }
}
