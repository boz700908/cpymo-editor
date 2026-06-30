using CpymoEditor.Core.Navigation;

namespace CpymoEditor.Tests.Navigation;

public sealed class EditorTabsTests
{
    [Fact]
    public void DefaultTabs_DefineTheSameSevenProductAreasForEveryPlatform()
    {
        IReadOnlyList<EditorTabDefinition> tabs = EditorTabs.Default;

        Assert.Equal(
            new[] { "Events", "Add", "Assets", "Config", "Problems", "Tools", "Source" },
            tabs.Select(tab => tab.Id));
        Assert.All(tabs, tab => Assert.False(string.IsNullOrWhiteSpace(tab.AccessibleName)));
    }
}
