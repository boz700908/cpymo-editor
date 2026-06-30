namespace CpymoEditor.Core.Navigation;

public static class EditorTabs
{
    public static IReadOnlyList<EditorTabDefinition> Default { get; } =
    [
        new("Events", "事件", "事件列表"),
        new("Add", "添加", "添加事件"),
        new("Assets", "素材", "素材库"),
        new("Config", "配置", "工程配置"),
        new("Problems", "问题", "问题列表"),
        new("Tools", "工具", "工具"),
        new("Source", "源码", "源码")
    ];
}
