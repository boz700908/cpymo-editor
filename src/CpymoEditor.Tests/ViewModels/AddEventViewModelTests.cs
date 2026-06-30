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

    [Fact]
    public void CreateEventCommand_BuildsDialogueEventFromGraphicalParameters()
    {
        var viewModel = new AddEventViewModel();
        AddEventTemplateViewModel dialogue = viewModel.Categories
            .SelectMany(category => category.Events)
            .Single(item => item.Name == "对话");

        dialogue.SelectCommand.Execute(null);
        viewModel.Speaker = "智也";
        viewModel.Text = "你好";
        viewModel.CreateEventCommand.Execute(null);

        Assert.NotNull(viewModel.CreatedEvent);
        Assert.Equal("#say 智也,你好", viewModel.CreatedEvent.RawText);
        Assert.Equal("已创建事件：对话", viewModel.StatusMessage);
    }
}
