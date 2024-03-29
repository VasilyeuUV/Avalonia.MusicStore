using Avalonia.MusicStore.ViewModels;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;

namespace Avalonia.MusicStore;

public partial class MusicStoreWindow : ReactiveWindow<MusicStoreViewModel>
{
    public MusicStoreWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.BuyMusicCommand.Subscribe(Close)));
    }
}