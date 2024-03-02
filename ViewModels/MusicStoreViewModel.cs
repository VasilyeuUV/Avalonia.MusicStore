using Avalonia.MusicStore.Models;
using iTunesSearch.Library.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace Avalonia.MusicStore.ViewModels
{
    /// <summary>
    /// ViewModel диалогового окна "MusicStore"
    /// </summary>
    public class MusicStoreViewModel : ViewModelBase
    {
        private string? _searchText;
        private bool _isBusy;
        private AlbumViewModel? _selectedAlbum;


        /// <summary>
        /// CTOR
        /// </summary>
        public MusicStoreViewModel()
        {
            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(400))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(DoSearch!);
        }

        public ObservableCollection<AlbumViewModel> SearchResults { get; } = new();

        public string? SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        public AlbumViewModel? SelectedAlbum
        {
            get => _selectedAlbum;
            set => this.RaiseAndSetIfChanged(ref _selectedAlbum, value);
        }

        /// <summary>
        /// Реализация поиска
        /// </summary>
        /// <param name="s"></param>
        private async void DoSearch(string s)
        {
            IsBusy = true;
            SearchResults.Clear();

            if (!string.IsNullOrWhiteSpace(s))
            {
                var albums = await AlbumModel.SearchAsync(s);

                foreach (var album in albums)
                {
                    var vm = new AlbumViewModel(album);
                    SearchResults.Add(vm);
                }
            }

            IsBusy = false;
        }
    }
}
