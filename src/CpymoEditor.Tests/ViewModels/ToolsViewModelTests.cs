using CpymoEditor.ViewModels;

namespace CpymoEditor.Tests.ViewModels;

public sealed class ToolsViewModelTests
{
    [Fact]
    public void Constructor_LoadsParityToolCommandsWithAccessibleNames()
    {
        var viewModel = new ToolsViewModel();

        Assert.Contains(viewModel.Tools, tool => tool.Name == "编译 YKM");
        Assert.Contains(viewModel.Tools, tool => tool.Name == "检查资源");
        Assert.Contains(viewModel.Tools, tool => tool.Name == "转换图片");
        Assert.Contains(viewModel.Tools, tool => tool.Name == "打包工程");
        Assert.All(viewModel.Tools, tool => Assert.False(string.IsNullOrWhiteSpace(tool.AccessibleName)));
        Assert.All(viewModel.Tools, tool => Assert.Equal("Windows / Android", tool.Platforms));
    }
}
