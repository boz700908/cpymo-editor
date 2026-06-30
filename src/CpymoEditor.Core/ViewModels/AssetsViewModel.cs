using System.Collections.ObjectModel;
using CpymoEditor.Core.Assets;

namespace CpymoEditor.ViewModels;

public sealed class AssetsViewModel
{
    public AssetsViewModel()
        : this(CreateSampleLibrary())
    {
    }

    public AssetsViewModel(AssetLibrary library)
    {
        foreach (AssetItem asset in library.Assets)
        {
            Assets.Add(ToRow(asset));
        }
    }

    public ObservableCollection<AssetRowViewModel> Assets { get; } = [];

    private static AssetLibrary CreateSampleLibrary()
    {
        AssetItem[] assets =
        [
            new(AssetKind.Background, "BG001_H", ".png", "bg/BG001_H.png", "bg/BG001_H.png"),
            new(AssetKind.Character, "hero_01", ".png", "chara/hero_01.png", "chara/hero_01.png"),
            new(AssetKind.Bgm, "BGM00", ".ogg", "bgm/BGM00.ogg", "bgm/BGM00.ogg"),
            new(AssetKind.SoundEffect, "click", ".ogg", "se/click.ogg", "se/click.ogg"),
            new(AssetKind.Voice, "voice_001", ".ogg", "voice/voice_001.ogg", "voice/voice_001.ogg")
        ];

        return new AssetLibrary(string.Empty, assets);
    }

    private static AssetRowViewModel ToRow(AssetItem asset)
    {
        string kind = asset.Kind switch
        {
            AssetKind.Background => "背景",
            AssetKind.Character => "立绘",
            AssetKind.Bgm => "音乐",
            AssetKind.SoundEffect => "音效",
            AssetKind.Voice => "语音",
            AssetKind.Video => "视频",
            AssetKind.SystemImage => "系统图片",
            AssetKind.Script => "脚本",
            _ => "素材"
        };

        string name = string.IsNullOrWhiteSpace(asset.Name)
            ? asset.RelativePath
            : asset.Name;

        return new AssetRowViewModel(
            kind,
            name,
            asset.RelativePath,
            kind + "：" + name + "，路径：" + asset.RelativePath);
    }
}
