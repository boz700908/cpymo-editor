using CpymoEditor.Core.Events;
using CpymoEditor.ViewModels;

namespace CpymoEditor.Tests.ViewModels;

public sealed class SourceViewModelTests
{
    [Fact]
    public void FromDocument_ExposesReadOnlySourceWithAccessibleSummary()
    {
        EventDocument document = PymoScriptParser.Parse(
            "script/start.txt",
            """
            #say 智也,你好
            #custom preserved
            """);

        SourceViewModel viewModel = SourceViewModel.FromDocument(document);

        Assert.Contains("#say 智也,你好", viewModel.SourceText);
        Assert.Contains("#custom preserved", viewModel.SourceText);
        Assert.True(viewModel.IsReadOnly);
        Assert.False(string.IsNullOrWhiteSpace(viewModel.AccessibleDescription));
    }
}
