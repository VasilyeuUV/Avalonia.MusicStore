﻿using Avalonia.MusicStore.Models;
using Avalonia.Media.Imaging;
using System.Threading.Tasks;
using ReactiveUI;               // в Авалонии использовать это, а не System.Bitmap из .NET

namespace Avalonia.MusicStore.ViewModels
{
    /// <summary>
    /// ViewModel диалогового окна "Album"
    /// </summary>
    public class AlbumViewModel : ViewModelBase
    {
        private readonly AlbumModel _album;
        private Bitmap? _cover;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="album"></param>
        public AlbumViewModel(AlbumModel album)
        {
            _album = album;
        }

        public string Artist => _album.Artist;

        public string Title => _album.Title;

        /// <summary>
        /// Для отображения картинок (Bitmap из AvaloniaUI)
        /// </summary>
        public Bitmap? Cover
        {
            get => _cover;
            private set => this.RaiseAndSetIfChanged(ref _cover, value);
        }

        /// <summary>
        /// Для асинхронного запуска в фоновом потоке, чтобы неблокировать UI-поток.
        /// </summary>
        /// <returns></returns>
        public async Task LoadCover()
        {
            await using (var imageStream = await _album.LoadCoverBitmapAsync())
            {
                Cover = await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 400));   // - DecodeToWidth, преобразует поток изображений для отображения через Avalonia UI.
            }
        }
    }
}
