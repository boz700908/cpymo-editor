using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CpymoEditor.ViewModels;

public sealed class AddEventViewModel : INotifyPropertyChanged
{
    private string _selectedEventName = string.Empty;

    public AddEventViewModel()
    {
        Categories =
        [
            Category("文本",
            [
                Template("对话", "添加角色名和台词"),
                Template("旁白", "添加没有角色名的叙述文本")
            ]),
            Category("图像",
            [
                Template("背景", "选择背景素材、转场和持续时间"),
                Template("立绘", "选择角色立绘、位置、图层和持续时间")
            ]),
            Category("声音",
            [
                Template("音乐", "选择背景音乐并设置是否循环"),
                Template("音效", "选择音效素材"),
                Template("语音", "选择语音素材")
            ]),
            Category("变量和跳转",
            [
                Template("变量赋值", "设置变量的值"),
                Template("跳转", "跳转到标签或脚本")
            ]),
            Category("选择",
            [
                Template("选择菜单", "添加多个选项和对应跳转")
            ]),
            Category("系统",
            [
                Template("等待", "等待指定毫秒数"),
                Template("等待按键", "等待玩家确认")
            ]),
            Category("YKM",
            [
                Template("宏或扩展", "保留或添加 YukimiScript 宏调用")
            ])
        ];
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<AddEventCategoryViewModel> Categories { get; }

    public string SelectedEventName
    {
        get => _selectedEventName;
        private set
        {
            if (_selectedEventName == value)
            {
                return;
            }

            _selectedEventName = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedEventName)));
        }
    }

    private AddEventCategoryViewModel Category(string name, AddEventTemplateViewModel[] events)
    {
        return new AddEventCategoryViewModel(name, events);
    }

    private AddEventTemplateViewModel Template(string name, string description)
    {
        return new AddEventTemplateViewModel(name, description, template => SelectedEventName = template.Name);
    }
}
