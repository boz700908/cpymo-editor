using System.ComponentModel;
using CpymoEditor.Core.Projects;

namespace CpymoEditor.ViewModels;

public sealed class EditorWorkspaceViewModel : INotifyPropertyChanged
{
    private EventsViewModel _events = new();
    private AssetsViewModel _assets = new();
    private ConfigViewModel _config = new();
    private ProblemsViewModel _problems = new();
    private SourceViewModel _source = new();
    private bool _canEdit;
    private string _statusMessage = "尚未打开工程";

    public event PropertyChangedEventHandler? PropertyChanged;

    public EventsViewModel Events
    {
        get => _events;
        private set => SetField(ref _events, value);
    }

    public AssetsViewModel Assets
    {
        get => _assets;
        private set => SetField(ref _assets, value);
    }

    public ConfigViewModel Config
    {
        get => _config;
        private set => SetField(ref _config, value);
    }

    public ProblemsViewModel Problems
    {
        get => _problems;
        private set => SetField(ref _problems, value);
    }

    public SourceViewModel Source
    {
        get => _source;
        private set => SetField(ref _source, value);
    }

    public bool CanEdit
    {
        get => _canEdit;
        private set => SetField(ref _canEdit, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetField(ref _statusMessage, value);
    }

    public void OpenProject(string projectDirectory)
    {
        ProjectWorkspace workspace = ProjectWorkspace.Open(projectDirectory);
        CanEdit = workspace.CanEdit;
        Assets = new AssetsViewModel(workspace.Assets);

        if (!workspace.CanEdit)
        {
            Problems = ProblemsViewModel.FromMessages(workspace.Messages);
            StatusMessage = workspace.Messages.FirstOrDefault() ?? "工程无法打开";
            return;
        }

        if (workspace.Events is not null)
        {
            Events = new EventsViewModel(workspace.Events);
            Source = SourceViewModel.FromDocument(workspace.Events);
        }

        if (workspace.Config is not null)
        {
            Config = new ConfigViewModel(workspace.Config);
        }

        Problems = new ProblemsViewModel();
        StatusMessage = "工程已打开";
    }

    private void SetField<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
