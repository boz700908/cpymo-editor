using System.Collections.ObjectModel;

namespace CpymoEditor.ViewModels;

public sealed class ToolsViewModel
{
    public ToolsViewModel()
    {
        Tools =
        [
            Tool("编译 YKM", "将 YukimiScript 编译为 PyMO 脚本"),
            Tool("检查资源", "检查资源引用和缺失文件"),
            Tool("转换图片", "转换图片、遮罩和适配格式"),
            Tool("打包工程", "生成可分发的 CPyMO 工程包"),
            Tool("用 CPyMO 运行", "通过外部 CPyMO 运行当前输出工程"),
            Tool("校验配置", "检查 gameconfig.txt 必填项和格式")
        ];
    }

    public ObservableCollection<ToolCommandViewModel> Tools { get; }

    private static ToolCommandViewModel Tool(string name, string description)
    {
        const string platforms = "Windows / Android";
        const string status = "等待打开工程";

        return new ToolCommandViewModel(
            name,
            description,
            platforms,
            status,
            name + "：" + description + "，平台：" + platforms + "，状态：" + status);
    }
}
