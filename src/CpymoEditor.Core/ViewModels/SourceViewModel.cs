using CpymoEditor.Core.Events;

namespace CpymoEditor.ViewModels;

public sealed class SourceViewModel
{
    public SourceViewModel()
        : this(
            """
            #say 智也,欢迎使用 CPyMO Editor
            #bg BG001_H,BG_FADE,500
            #wait 1000
            """,
            "高级源码视图，只读显示当前脚本内容")
    {
    }

    private SourceViewModel(string sourceText, string accessibleDescription)
    {
        SourceText = sourceText;
        AccessibleDescription = accessibleDescription;
    }

    public string SourceText { get; }

    public bool IsReadOnly => true;

    public string AccessibleDescription { get; }

    public static SourceViewModel FromDocument(EventDocument document)
    {
        string source = PymoScriptWriter.Write(document);
        string description = "高级源码视图，只读显示 " + document.Path + " 的脚本内容";
        return new SourceViewModel(source, description);
    }
}
