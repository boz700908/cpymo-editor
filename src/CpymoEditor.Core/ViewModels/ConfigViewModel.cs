using System.ComponentModel;
using System.Windows.Input;
using CpymoEditor.Core.Configuration;

namespace CpymoEditor.ViewModels;

public sealed class ConfigViewModel : INotifyPropertyChanged
{
    private readonly GameConfigDocument _document;
    private string _gameTitle;
    private string _startScript;
    private string _scriptType;
    private string _backgroundFormat;
    private string _saveStatus = string.Empty;
    private string _serializedConfig = string.Empty;

    public ConfigViewModel()
        : this(GameConfigDocument.Parse("scripttype,pymo\nstartscript,start\nbgformat,.jpg\n"))
    {
    }

    public ConfigViewModel(GameConfigDocument document)
    {
        _document = document;
        _gameTitle = document.GetValue("gametitle") ?? string.Empty;
        _startScript = document.GetValue("startscript") ?? "start";
        _scriptType = document.GetValue("scripttype") ?? "pymo";
        _backgroundFormat = document.GetValue("bgformat") ?? ".jpg";
        SaveCommand = new RelayCommand(Save);
        _serializedConfig = document.Write();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string GameTitle
    {
        get => _gameTitle;
        set => SetField(ref _gameTitle, value);
    }

    public string StartScript
    {
        get => _startScript;
        set => SetField(ref _startScript, value);
    }

    public string ScriptType
    {
        get => _scriptType;
        set => SetField(ref _scriptType, value);
    }

    public string BackgroundFormat
    {
        get => _backgroundFormat;
        set => SetField(ref _backgroundFormat, value);
    }

    public string SaveStatus
    {
        get => _saveStatus;
        private set => SetField(ref _saveStatus, value);
    }

    public string SerializedConfig
    {
        get => _serializedConfig;
        private set => SetField(ref _serializedConfig, value);
    }

    public ICommand SaveCommand { get; }

    private void Save()
    {
        _document.SetValue("scripttype", ScriptType);
        _document.SetValue("startscript", StartScript);
        _document.SetValue("gametitle", GameTitle);
        _document.SetValue("bgformat", BackgroundFormat);
        SerializedConfig = _document.Write();
        SaveStatus = "配置已更新";
    }

    private void SetField(ref string field, string value, [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        if (field == value)
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
