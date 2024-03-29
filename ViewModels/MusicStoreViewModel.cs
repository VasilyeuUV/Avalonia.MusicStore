﻿using Avalonia.MusicStore.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

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
        private CancellationTokenSource? _cancellationTokenSource;  // - позволит отменить выполнение метода по загрузке обложек альбомов.

        /// <summary>
        /// CTOR
        /// </summary>
        public MusicStoreViewModel()
        {
            // Инициализации реактивной команды Покупки альбома
            BuyMusicCommand = ReactiveCommand.Create(() =>
            {
                return SelectedAlbum;
            });

            // Запуск поиска всякий раз, как меняется текст
            this.WhenAnyValue(x => x.SearchText)                // - возвращает наблюдаемое свойство каждый раз, когда меняется строка поиска
                .Throttle(TimeSpan.FromMilliseconds(400))       // - немного подождать, пока пользователь не перестанет вводите текст
                .ObserveOn(RxApp.MainThreadScheduler)           // - обеспечения вызова подписанного метода в UI-потоке
                .Subscribe(DoSearch!);                          // - вызов метода DoSearch для каждого наблюдаемого события
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


        //########################################################################################
        #region COMMANDS

        /// <summary>
        /// Команда для покупки музыкального альбома
        /// </summary>
        public ReactiveCommand<Unit, AlbumViewModel?> BuyMusicCommand { get; }



        #endregion // COMMANDS


        //########################################################################################
        #region PRIVATE METHODS

        /// <summary>
        /// Реализация поиска
        /// </summary>
        /// <param name="s"></param>
        private async void DoSearch(string s)
        {
            IsBusy = true;
            SearchResults.Clear();

            // установка cancellation token
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            if (!string.IsNullOrWhiteSpace(s))
            {
                var albums = await AlbumModel.SearchAsync(s);

                foreach (var album in albums)
                {
                    var vm = new AlbumViewModel(album);
                    SearchResults.Add(vm);
                }

                if (!cancellationToken.IsCancellationRequested)
                {
                    LoadCovers(cancellationToken);
                }
            }

            IsBusy = false;
        }


        /// <summary>
        /// Метод, который сможет запускать загрузку обложек альбомов при изменении результатов поиска (асинхронный, отменяемый)
        /// </summary>
        /// <param name="cancellationToken"></param>
        private async void LoadCovers(CancellationToken cancellationToken)
        {
            foreach (var album in SearchResults.ToList())
            {
                await album.LoadCover();

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        #endregion // PRIVATE METHODS
    }
}
