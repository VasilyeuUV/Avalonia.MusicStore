using Avalonia.MusicStore.Models;

namespace Avalonia.MusicStore.ViewModels
{
    /// <summary>
    /// ViewModel диалогового окна "Album"
    /// </summary>
    public class AlbumViewModel : ViewModelBase
    {
        private readonly AlbumModel _album;

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
    }
}
